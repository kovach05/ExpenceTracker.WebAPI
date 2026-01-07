using ExpenseTracker.Infrastructure.Models;

namespace ExpenceTracker.WebAPI.JWT;

public interface IJwtService
{
    string GenerateToken(User user, IList<string> roles);
}