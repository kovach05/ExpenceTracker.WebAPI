using ExpenceTracker.Apllication.DTOs;

namespace ExpenceTracker.Apllication.Interfaces;

public interface IAuthService
{
    Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterRequest request);
    Task<(bool IsSuccess, string TokenOrMessage)> LoginAsync(LoginRequest request);
}