using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(
    ILogger<UsersController> logger,
    IUsersService usersService) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.ManageEmployees)]
    [ProducesResponseType(typeof(List<UserPartialViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        try
        {
            var users = await usersService.GetAllAsync(cancellationToken);

            if (users == null || users.Count == 0)
            {
                return NoContent();
            }

            return Ok(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get users due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("{id}")]
    [HasPermission(UserPermission.ManageEmployees)]
    [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var user = await usersService.GetByIdAsync(id, cancellationToken);

            if (user == null)
            {
                return NotFound($"No user with Id={id} found.");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get user due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}
