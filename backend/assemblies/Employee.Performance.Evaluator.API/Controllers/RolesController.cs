using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Roles;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(
    ILogger<RolesController> logger,
    IRolesService rolesService) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.ManageRoles)]
    [ProducesResponseType(typeof(List<Role>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        try
        {
            var roles = await rolesService.GetAllRolesAsync(cancellationToken);

            if (roles == null || roles.Count == 0)
            {
                return NotFound("No roles found.");
            }

            return Ok(roles);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get roles due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("{id}")]
    [HasPermission(UserPermission.ManageRoles)]
    [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoleById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var role = await rolesService.GetByIdAsync(id, cancellationToken);

            if (role == null)
            {
                return NotFound($"No role with Id={id} found.");
            }

            return Ok(role);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get role due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost]
    [HasPermission(UserPermission.ManageRoles)]
    [ProducesResponseType(typeof(Role), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRole([FromBody] AddUpdateRoleRequest addUpdateRoleRequest, CancellationToken cancellationToken)
    {
        if (addUpdateRoleRequest == null)
        {
            return BadRequest("Invalid role data.");
        }

        try
        {
            var createdRole = await rolesService.CreateRoleAsync(addUpdateRoleRequest, cancellationToken);
            return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to create new role");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create role due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("{id}")]
    [HasPermission(UserPermission.ManageRoles)]
    [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] AddUpdateRoleRequest addUpdateRoleRequest, CancellationToken cancellationToken)
    {
        if (addUpdateRoleRequest == null)
        {
            return BadRequest("Invalid role data.");
        }

        try
        {
            var updatedRole = await rolesService.UpdateRoleAsync(id, addUpdateRoleRequest, cancellationToken);
            return Ok(updatedRole);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to update role");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update role due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpDelete("{id}")]
    [HasPermission(UserPermission.ManageRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cancellationToken)
    {
        try
        {
            await rolesService.DeleteRoleAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to delete role");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete role due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("{id}/permissions")]
    [HasPermission(UserPermission.ManageRoles)]
    [ProducesResponseType(typeof(Role), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRolePermissions(int id, [FromBody] List<int> permissionIds, CancellationToken cancellationToken)
    {
        if (permissionIds == null || permissionIds.Count == 0)
        {
            return BadRequest("Invalid permission data.");
        }

        try
        {
            var updatedRole = await rolesService.UpdateRolePermissionsAsync(id, permissionIds, cancellationToken);
            return Ok(updatedRole);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to update role permissions");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update role permissions due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}
