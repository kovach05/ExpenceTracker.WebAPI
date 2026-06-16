using System.Security.Claims;
using ExpenceTracker.Infrastructure;
using ExpenceTracker.WebAPI.DTOs;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenceTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public AccountController(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var userId = GetUserId();
        var accounts = await _dbContext.Accounts
            .Where(a => a.UserId == userId && a.IsActive)
            .ToListAsync();
        
        return Ok(accounts);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        var userId = GetUserId();

        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Balance = request.InitialBalance,
            IsSmart = request.IsSmart,
            Currency = request.Currency, 
            IsActive = true,
            DateCreated = DateTime.UtcNow
        };
    
        _dbContext.Accounts.Add(account);
        await _dbContext.SaveChangesAsync();
    
        return Ok(account);
    }
    
    [HttpDelete("{id}")] // Важливо: тут має бути {id}
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        var account = await _dbContext.Accounts.FindAsync(id);
        if (account == null)
        {
            return NotFound();
        }

        _dbContext.Accounts.Remove(account);
        await _dbContext.SaveChangesAsync();

        return NoContent(); // Повертає 204
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(userIdString!);
    }
}