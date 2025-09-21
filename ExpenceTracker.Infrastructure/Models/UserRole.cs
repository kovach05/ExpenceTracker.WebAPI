using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Models;

public class UserRole : IdentityUserRole<string>
{
    public int Id { get; set; }
    public string Name { get; set; }
}   