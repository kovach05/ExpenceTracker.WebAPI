using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Models;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
}