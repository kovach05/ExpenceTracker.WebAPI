using System.Security.Claims;
using ExpenceTracker.Infrastructure;
using ExpenceTracker.WebAPI.DTOs;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenceTracker.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BudgetsController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public BudgetsController(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpPost("distribute")]
    public async Task<IActionResult> DistributeIncome([FromBody] DistributeIncomeRequest request)
    {
        if (request.TotalIncome <= 0) return BadRequest("Сума доходу повинна бути більшою за 0");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        
        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        decimal needsShare = request.TotalIncome * 0.50m;
        decimal wantsShare = request.TotalIncome * 0.30m;
        decimal savingsShare = request.TotalIncome * 0.20m;

        await UpdateOrCreateBudget(userId, "Needs", needsShare, currentMonth);
        await UpdateOrCreateBudget(userId, "Wants", wantsShare, currentMonth);
        await UpdateOrCreateBudget(userId, "Savings", savingsShare, currentMonth);

        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Дохід успішно розподілено за системою 50/30/20" });
    }
    
    [HttpPost("expense")]
    public async Task<IActionResult> RecordExpense([FromBody] BudgetExpenseRequest request)
    {
        if (request.Amount <= 0) return BadRequest("Сума витрати повинна бути більшою за 0");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        
        var existingBudget = await _dbContext.MonthlyBudgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BudgetType == request.BudgetType && b.MonthYear == currentMonth);

        if (existingBudget == null)
        {
            _dbContext.MonthlyBudgets.Add(new MonthlyBudget()
            {
                UserId = userId,
                BudgetType = request.BudgetType,
                MaxAmount = 0,
                CurrentSpent = request.Amount,
                MonthYear = currentMonth
            });
        }
        else
        {
            existingBudget.CurrentSpent += request.Amount;
        }

        await _dbContext.SaveChangesAsync();
        return Ok(new { message = "Витрату успішно враховано в розумному бюджеті" });
    }
    
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentBudgets()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();
        
        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var budgets = await _dbContext.MonthlyBudgets
            .Where(b => b.UserId == userId && b.MonthYear == currentMonth)
            .ToListAsync();

        return Ok(budgets);
    }
    
    private async Task UpdateOrCreateBudget(string userId, string type, decimal amount, DateTime monthYear)
    {
        monthYear = DateTime.SpecifyKind(monthYear, DateTimeKind.Utc);

        var existingBudget = await _dbContext.MonthlyBudgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.BudgetType == type && b.MonthYear == monthYear);

        if (existingBudget == null)
        {
            _dbContext.MonthlyBudgets.Add(new MonthlyBudget()
            {
                UserId = userId,
                BudgetType = type,
                MaxAmount = amount,
                CurrentSpent = 0,
                MonthYear = monthYear
            });
        }
        else
        {
            existingBudget.MaxAmount += amount; 
        }
    }
}

public class BudgetExpenseRequest
{
    public string BudgetType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}