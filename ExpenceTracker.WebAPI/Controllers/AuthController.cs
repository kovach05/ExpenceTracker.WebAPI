using ExpenceTracker.Apllication.DTOs;
using ExpenceTracker.Apllication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExpenceTracker.Apllication.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var (isSuccess, message) = await _authService.RegisterAsync(request);

        if (!isSuccess)
            return BadRequest(message);

        return Ok(message);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var (isSuccess, tokenOrMessage) = await _authService.LoginAsync(request);

        if (!isSuccess)
            return Unauthorized(new { Message = tokenOrMessage });

        return Ok(new { Token = tokenOrMessage });
    }

}