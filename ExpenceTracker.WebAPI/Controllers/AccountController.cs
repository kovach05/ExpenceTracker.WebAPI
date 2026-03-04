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

    /// oтримати всі рахунки користувача
    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var userId = GetUserId();
        var accounts = await _dbContext.Accounts
            .Where(a => a.UserId == userId && a.IsActive)
            .ToListAsync();
        
        return Ok(accounts);
    }
    
//створення рахунку
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
            IsActive = true
        };
        
        _dbContext.Accounts.Add(account);
        await _dbContext.SaveChangesAsync();
        
        return Ok(account);
    }

    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.Parse(userIdString!);
    }
}