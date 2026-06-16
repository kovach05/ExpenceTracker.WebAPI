using System.Security.Claims;
using ExpenceTracker.WebAPI.DTOs;
using ExpenceTracker.WebAPI.Services;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenceTracker.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Доступ тільки для авторизованих користувачів
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // 1. Отримати всі категорії поточного користувача
    [HttpGet]
    public async Task<IActionResult> GetMyCategories()
    {
        var userId = GetUserId();
        var categories = await _categoryService.GetUserCategoriesAsync(userId);
        
        // Мапимо моделі БД на DTO
        var response = categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type,
            BudgetType = c.BudgetType // 🟢 ДОДАНО: повертаємо BudgetType на фронтенд
        });

        return Ok(response);
    }

    // 2. Створити нову категорію
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = GetUserId();
        
        try 
        {
            // 🟢 ОНОВЛЕНО: Передаємо request.BudgetType четвертим параметром у сервіс
            var category = await _categoryService.CreateCategoryAsync(userId, request.Name, request.Type, request.BudgetType);
            
            return CreatedAtAction(nameof(GetMyCategories), new { id = category.Id }, new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                BudgetType = category.BudgetType // 🟢 ДОДАНО: повертаємо створений BudgetType
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // 3. "Видалити" категорію (Soft Delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();
        var result = await _categoryService.DeactivateCategoryAsync(userId, id);
        
        if (!result) return NotFound(new { message = "Категорію не знайдено або доступ заборонено" });

        return NoContent();
    }

    // Хелпер-метод для отримання UserId з токена
    private Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString))
        {
            userIdString = User.FindFirstValue("sub");
        }

        if (Guid.TryParse(userIdString, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException($"Невірний формат ID користувача у токені: {userIdString}");
    }
}