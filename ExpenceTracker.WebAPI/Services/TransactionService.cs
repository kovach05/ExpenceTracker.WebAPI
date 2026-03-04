using ExpenceTracker.Infrastructure;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenceTracker.WebAPI.Services;

public class TransactionService
{
    private readonly ExpenseTrackerDbContext _dbContext;

    public TransactionService(ExpenseTrackerDbContext context)
    {
        _dbContext = context;
    }

    public async Task<List<Transaction>> GetUserTransactionsAsync(Guid userId)
    {
        return await _dbContext.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.IsActive)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
        
    }
    
    public async Task<Transaction> CreateTransactionAsync(Guid userId, decimal amount, Guid categoryId, Guid accountId, string? comment)
    {
        // 1. Шукаємо рахунок і перевіряємо, чи він належить користувачу
        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId);

        if (account == null)
            throw new Exception("Рахунок не знайдено або доступ заборонено.");

        // 2. Шукаємо категорію
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new Exception("Категорію не знайдено.");

        if (category.UserId != userId)
            throw new Exception("Доступ заборонено до цієї категорії.");

        // --- НОВА ЛОГІКА БАЛАНСУ ---
        // 3. Змінюємо баланс рахунку в залежності від типу категорії
        if (category.Type == "Income") // Якщо це дохід
        {
            account.Balance += amount;
        }
        else if (category.Type == "Expense") // Якщо це витрата
        {
            // Можна додати перевірку: якщо на рахунку недостатньо грошей
            // if (account.Balance < amount) throw new Exception("Недостатньо коштів на рахунку.");
        
            account.Balance -= amount;
        }

        // 4. Створення об'єкта транзакції (тепер з AccountId)
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = categoryId,
            AccountId = accountId, // Додаємо зв'язок з рахунком
            Amount = amount,
            Date = DateTime.UtcNow,
            Description = comment,
            Type = category.Type,
            IsActive = true
        };

        // Зберігаємо і транзакцію, і оновлений баланс рахунку (EF зробить це в одній транзакції)
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        return transaction;
    }
}