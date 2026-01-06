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
    // Навігаційні властивості (Relationships)
    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
}