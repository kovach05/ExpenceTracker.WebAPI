using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Infrastructure.Models;

public class Goal
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    public string Name { get; set; }
        
    [Required]
    public decimal Target { get; set; }
        
    public decimal Current { get; set; }
        
    public string Color { get; set; }
    
    public string UserId { get; set; }
}