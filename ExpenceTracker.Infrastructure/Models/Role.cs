using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Models;

public class Role : IdentityRole<Guid>
{
    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
}