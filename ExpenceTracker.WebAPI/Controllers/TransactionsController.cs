using System.Security.Claims;
using ExpenceTracker.WebAPI.DTOs;
using ExpenceTracker.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenceTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Тільки для залогінених користувачів
public class TransactionsController : ControllerBase
{
   private readonly TransactionService _transactionService;

   public TransactionsController(TransactionService transactionService)
   {
      _transactionService = transactionService;
   }

   [HttpPost]
   public async Task<IActionResult> Create(CreateTransactionRequest request)
   {
      // Отримуємо UserId з токена авторизації
      var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
      var result = await _transactionService.CreateTransactionAsync(
         userId, request.Amount, request.CategoryId, request.Comment);
            
      return Ok(result);
   }
}