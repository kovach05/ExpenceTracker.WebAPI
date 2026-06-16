using System.Security.Claims;
using ExpenceTracker.Infrastructure;
using ExpenceTracker.WebAPI.DTOs;
using ExpenceTracker.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenceTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
   private readonly TransactionService _transactionService;
   private readonly ExpenseTrackerDbContext _dbContext;

   public TransactionsController(TransactionService transactionService, ExpenseTrackerDbContext dbContext)
   {
      _transactionService = transactionService;
      _dbContext = dbContext;
   }

   [HttpPost]
   public async Task<IActionResult> Create([FromBody] CreateTransactionRequest request)
   {
      if (!ModelState.IsValid) return BadRequest(ModelState);

      var userId = GetUserId();
      try
      {
         var t = await _transactionService.CreateTransactionAsync(
            userId, 
            request.Amount, 
            request.CategoryId, 
            request.AccountId,
            request.Comment);
         
         
         var category = await _dbContext.Categories.FindAsync(request.CategoryId);
         
         if (category != null && t.Type.ToString().ToLower() == "expense")
         {
             var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
             string budgetType = DetermineBudgetType(category.Name);
             
             var activeBudget = await _dbContext.MonthlyBudgets
                 .FirstOrDefaultAsync(b => b.UserId == userId.ToString() && b.BudgetType == budgetType && b.MonthYear == currentMonth);

             if (activeBudget != null)
             {
                 activeBudget.CurrentSpent += t.Amount;
                 await _dbContext.SaveChangesAsync();
             }
         }
         
         return Ok(new TransactionResponse
         {
            Id = t.Id,
            Amount = t.Amount,
            Description = t.Description ?? "",
            Date = t.Date,
            Type = t.Type
         });
      }
      catch (Exception ex)
      {
         return BadRequest(new { message = ex.Message });
      }
   }
   
   [HttpGet]
   public async Task<IActionResult> GetTransactions()
   {
      var userId = GetUserId();
    
      var transactions = await _dbContext.Transactions
         .Include(t => t.Account)    
         .Include(t => t.Category)   
         .Where(t => t.Account.UserId == userId)
         .Select(t => new {
            t.Id,
            t.Amount,
            Comment = t.Description ?? "",
            t.Date,
            CategoryName = t.Category.Name,
            CategoryType = t.Category.Type.ToString(),
            AccountName = t.Account.Name,
            Currency = t.Account.Currency
         })
         .OrderByDescending(t => t.Date)
         .ToListAsync();

      return Ok(transactions);
   }
   
   private string DetermineBudgetType(string categoryName)
   {
      string[] needsCategories = { "Продукти", "Комуналка", "Оренда", "Транспорт", "Ліки", "Аптека", "Комунальні", "Їжа" };
      string[] wantsCategories = { "Кафе", "Ресторани", "Розваги", "Кіно", "Шопінг", "Одяг", "Таксі", "Хобі" };

      if (needsCategories.Contains(categoryName)) return "Needs";
      if (wantsCategories.Contains(categoryName)) return "Wants";
    
      return "Savings";
   }

   private Guid GetUserId()
   {
      var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
      
      if (Guid.TryParse(userIdString, out var userId))
      {
          return userId;
      }
      
      throw new UnauthorizedAccessException("Користувача не ідентифіковано");
   }
}