namespace ExpenceTracker.WebAPI.DTOs;

public class CreateTransferRequest
{
    public Guid FromAccountId { get; set; }
    public Guid ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Comment { get; set; }
}