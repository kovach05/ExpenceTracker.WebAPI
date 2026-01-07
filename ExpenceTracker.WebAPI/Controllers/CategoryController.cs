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
        
        // Мапимо моделі БД на DTO (можна використовувати AutoMapper, але для диплома можна і вручну)
        var response = categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type
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
            var category = await _categoryService.CreateCategoryAsync(userId, request.Name, request.Type);
            
            return CreatedAtAction(nameof(GetMyCategories), new { id = category.Id }, new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type
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
        // Використовуємо FindFirstValue — це зручний метод розширення
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Якщо стандартний NameIdentifier не спрацював, спробуємо знайти за назвою "sub"
        if (string.IsNullOrEmpty(userIdString))
        {
            userIdString = User.FindFirstValue("sub");
        }

        if (Guid.TryParse(userIdString, out var userId))
        {
            return userId;
        }

        // Якщо ми тут, значить у токені замість ID лежить щось інше (наприклад, Email)
        throw new UnauthorizedAccessException($"Невірний формат ID користувача у токені: {userIdString}");
    }
}