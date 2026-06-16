using System.Collections;

namespace ExpenseTracker.Infrastructure.Models;

public class Category
{
    public Guid Id {get;set;} =  Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name {get;set;} = string.Empty;
    public string Type { get; set; } = "expence";
    public bool IsActive {get;set;} = true;
    
    public string? BudgetType { get; set; }
}