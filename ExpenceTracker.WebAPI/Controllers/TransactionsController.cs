using System.Security.Claims;
using ExpenceTracker.WebAPI.DTOs;
using ExpenceTracker.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ExpenceTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
   private readonly TransactionService _transactionService;

   public TransactionsController(TransactionService transactionService)
   {
      _transactionService = transactionService;
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

         // Повертаємо результат
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
   public async Task<IActionResult> GetMyTransactions()
   {
      var userId = GetUserId();
      var transactions = await _transactionService.GetUserTransactionsAsync(userId);

      var response = transactions.Select(t => new TransactionResponse
      {
         Id = t.Id,
         Amount = t.Amount,
         Description = t.Description ?? "",
         Date = t.Date,
         Type = t.Type,
         // Якщо в моделі Transaction є зв'язок з Category, можна дістати ім'я:
         CategoryName = t.Category?.Name ?? "Без категорії" 
      });

      return Ok(response);
   }

   private Guid GetUserId()
   {
      var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
      return Guid.Parse(userIdString!);
   }
}