using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions.Auth;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException)
        {
            logger.LogError(ex, "Login failed for user {Email}", request.Email);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to login due to an unexpected error: {Message}", ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            logger.LogError(ex, "Registration failed for user {Email}", request.Email);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to register due to an unexpected error: {Message}", ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}
