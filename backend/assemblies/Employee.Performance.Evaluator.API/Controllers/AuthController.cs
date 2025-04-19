using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    IUserGetter userGetter,
    ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Login failed for user {Username}", request.Username);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Registration failed for user {Username}", request.Username);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("users/me")]
    public Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var user = userGetter.GetCurrentUserOrThrow();
            return Task.FromResult<IActionResult>(Ok(user));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get current user");
            return Task.FromResult<IActionResult>(BadRequest(ex.Message));
        }
    }
}
