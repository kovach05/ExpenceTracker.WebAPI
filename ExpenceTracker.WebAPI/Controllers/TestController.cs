using Microsoft.AspNetCore.Mvc;

namespace ExpenceTracker.Apllication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API is working!");
    }
}