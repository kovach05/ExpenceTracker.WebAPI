using ExpenceTracker.Infrastructure;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenceTracker.WebAPI.Services;

public class CategoryService
{
    private readonly ExpenseTrackerDbContext _dbContext;
    
    public CategoryService(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Category>> GetUserCategoriesAsync(Guid userId)
    {
        return await _dbContext.Categories
            .Where(c => c.UserId == userId && c.IsActive)
            .ToListAsync();
    }

    public async Task<Category> CreateCategoryAsync(Guid userId, string name, string type)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Type = type,
            IsActive = true
        };
        
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();
        return category;
    }
    
    public async Task<bool> DeactivateCategoryAsync(Guid userId, Guid categoryId)
    {
        // Шукаємо категорію, яка належить саме цьому юзеру
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

        if (category == null)
        {
            return false; // Категорія не знайдена або належить іншому юзеру
        }

        // Замість видалення — деактивуємо
        category.IsActive = false;

        _dbContext.Categories.Update(category);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }
}