using System.ComponentModel.DataAnnotations;

namespace ExpenceTracker.WebAPI.DTOs;

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    // 🟢 ВИПРАВЛЕНО: "Expense" замість "expence", щоб збігалося з React
    public string Type { get; set; } = "Expense"; 
    
    // 🟢 ДОДАНО: Прив'язка до кошика 50/30/20 (Needs, Wants, Savings)
    public string? BudgetType { get; set; } 
}

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    
    // 🟢 ДОДАНО: Щоб фронтенд бачив, до якого кошика належить категорія
    public string? BudgetType { get; set; }
}