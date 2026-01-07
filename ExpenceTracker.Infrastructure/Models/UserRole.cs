using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Infrastructure.Models;

// ОБОВ'ЯЗКОВО вказуємо <Guid> тут
public class UserRole : IdentityUserRole<Guid> 
{
    // Навігаційні властивості (опціонально, але корисно)
    public virtual User User { get; set; }
    public virtual Role Role { get; set; }
}