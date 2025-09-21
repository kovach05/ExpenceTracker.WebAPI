using ExpenceTracker.Apllication.DTOs;
using ExpenceTracker.Apllication.Interfaces;
using ExpenceTracker.WebAPI.JWT;
using ExpenseTracker.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ExpenceTracker.Apllication.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<(bool IsSuccess, string Message)> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrEmpty(request.FirstName))
            return (false, "First name is required");

        if (await _userManager.FindByNameAsync(request.UserName) != null)
            return (false, "Username already exists");

        var user = new User
        {
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

        return (true, "User registered successfully");
    }
    
    public async Task<(bool IsSuccess, string TokenOrMessage)> LoginAsync(LoginRequest request)
    {
        // шукаємо користувача по username
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
            return (false, "Invalid username or password");

        // перевіряємо пароль
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
            return (false, "Invalid username or password");

        // генеруємо JWT
        var token = _jwtService.GenerateToken(user);

        return (true, token);
    }

}