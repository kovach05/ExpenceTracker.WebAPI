using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenceTracker.WebAPI.Services;

public class TransactionService
{
    private readonly ExpenseTrackerDbContext _context;

    public TransactionService(ExpenseTrackerDbContext context)
    {
        _context = context;
    }

    // 1. Змінюємо ім'я параметра з Id на categoryId для ясності
    public async Task<Transaction> CreateTransactionAsync(Guid userId, decimal amount, Guid categoryId, string? comment)
    {
        // 2. Тепер шукаємо категорію за вказаним categoryId
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new Exception("Категорію не знайдено.");

        if (category.UserId != userId)
            throw new Exception("Доступ заборонено: категорія належить іншому користувачу.");

        if (!category.IsActive)
            throw new Exception("Ця категорія деактивована. Оберіть іншу.");

        // 3. Створення об'єкта транзакції
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),    // Це новий ID самої транзакції
            UserId = userId,
            CategoryId = categoryId, // Тепер це ім'я збігається з параметром зверху
            Amount = amount,
            Date = DateTime.UtcNow,
            Description = comment,   // Зверніть увагу: у вашій моделі поле називається Description, а не Comment
            Type = category.Type     // Копіюємо тип (income/expense) з категорії
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }
}