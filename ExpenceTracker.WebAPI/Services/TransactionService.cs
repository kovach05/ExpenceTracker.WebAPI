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
        var account = await _dbContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId);

        if (account == null)
            throw new Exception("Рахунок не знайдено або доступ заборонено.");
        
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new Exception("Категорію не знайдено.");

        if (category.UserId != userId)
            throw new Exception("Доступ заборонено до цієї категорії.");
        
        if (category.Type == "Income")
        {
            account.Balance += amount;
        }
        else if (category.Type == "Expense")
        {
        
            account.Balance -= amount;
        }
        
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = categoryId,
            AccountId = accountId,
            Amount = amount,
            Date = DateTime.UtcNow,
            Description = comment,
            Type = category.Type,
            IsActive = true
        };
        
        _dbContext.Transactions.Add(transaction);
        await _dbContext.SaveChangesAsync();

        return transaction;
    }
}