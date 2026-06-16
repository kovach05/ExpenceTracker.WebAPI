using ExpenceTracker.Infrastructure;
using ExpenseTracker.Infrastructure;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenceTracker.WebAPI.Services;

public class TransferService
{
    private readonly ExpenseTrackerDbContext _dbContext;
    
    public TransferService(ExpenseTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateTransferAsync(Guid userId, Guid fromId, Guid toId, decimal amount, string comment)
    {

        var fromAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == fromId && a.UserId == userId);
        var toAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == toId && a.UserId == userId);

        if (fromAccount == null || toAccount == null)
            throw new Exception("One of the accounts cannot be found.");

        if (fromAccount.Balance < amount)
            throw new Exception("Insufficient funds in the sender's account.");
        
        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        var transfer = new Transfer
        {
            UserId = userId,
            FromAccountId = fromId,
            ToAccountId = toId,
            Amount = amount,
            Comment = comment
        };
        
        _dbContext.Transfers.Add(transfer);
        
        await _dbContext.SaveChangesAsync();
    }
}