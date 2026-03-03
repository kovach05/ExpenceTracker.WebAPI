using System.ComponentModel.DataAnnotations;

namespace ExpenceTracker.WebAPI.DTOs;

public class CreateTransactionRequest
{
    [Required(ErrorMessage = "Сумма є обов'язковою")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Сума повинна бути більшою за 0")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "Категорія є обов'язковою")]
    public Guid CategoryId { get; set; }
    
    [Required]
    public Guid AccountId { get; set; }
    
    [MaxLength(200, ErrorMessage = "Коментар не може перевищувати 200 символів")]
    public string? Comment { get; set; }
    
    public DateTime? Date { get; set; }
}