using System.Security.Claims;
using ExpenceTracker.Infrastructure;
using ExpenceTracker.WebAPI.DTOs;
using ExpenceTracker.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Важливо для Include та ToListAsync

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
    
      // Отримуємо транзакції, які належать рахункам поточного користувача
      var transactions = await _dbContext.Transactions
         .Include(t => t.Account)    
         .Include(t => t.Category)   
         .Where(t => t.Account.UserId == userId) // Фільтруємо по власнику рахунку
         .Select(t => new {
            t.Id,
            t.Amount,
            Comment = t.Description ?? "", // Переконайся, що в моделі Description або Comment
            t.Date,
            CategoryName = t.Category.Name,
            CategoryType = t.Category.Type.ToString(),
            AccountName = t.Account.Name,
            Currency = t.Account.Currency // Тепер React побачить валюту
         })
         .OrderByDescending(t => t.Date)
         .ToListAsync();

      return Ok(transactions);
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