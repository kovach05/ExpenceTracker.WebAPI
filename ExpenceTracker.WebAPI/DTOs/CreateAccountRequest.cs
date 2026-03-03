namespace ExpenceTracker.WebAPI.DTOs;

public class CreateAccountRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal InitialBalance { get; set; }
}