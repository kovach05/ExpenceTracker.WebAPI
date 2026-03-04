namespace ExpenseTracker.Infrastructure.Models;

public class Transfer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    
    public Guid FromAccountId { get; set; } // звідки відправляти
    public virtual Account FromAccount { get; set; } = null!;
    
    public Guid ToAccountId { get; set; } // куди прийдуть
    public virtual Account ToAccount { get; set; } = null!;
    
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Comment { get; set; }
}