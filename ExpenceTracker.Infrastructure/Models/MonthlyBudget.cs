using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Infrastructure.Models;

public class MonthlyBudget
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    [MaxLength(20)]
    public string BudgetType { get; set; }

    [Required]
    public decimal MaxAmount { get; set; }

    [Required]
    public decimal CurrentSpent { get; set; }

    [Required]
    public DateTime MonthYear { get; set; }
}