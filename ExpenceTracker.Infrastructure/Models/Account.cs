namespace ExpenseTracker.Infrastructure.Models;

public class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0;
    public Guid UserId { get; set; }
    public string Currency { get; set; } = "UAH";
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}