using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Infrastructure.Models;

public class Transaction
{
    public Guid Id {get;set;} = Guid.NewGuid();
    public Guid UserId {get;set;}
    public Guid CategoryId {get;set;}
    public decimal Amount {get;set;}
    public string? Description {get;set;}
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public string? Type {get;set;}
    
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
    public bool IsActive {get;set;} = true;
    
    public Guid AccountId {get;set;}

    [ForeignKey("AccountId")]
    public virtual Account Account { get; set; } = null!;
}