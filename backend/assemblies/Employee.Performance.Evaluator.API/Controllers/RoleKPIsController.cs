using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.RoleKPI;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Core.Exceptions;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleKPIsController(
    ILogger<RoleKPIsController> logger,
    IRoleKPIsService roleKPIsService) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.ManageRoles)]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(typeof(List<RoleKPIViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllRoleKPIs(CancellationToken cancellationToken)
    {
        try
        {
            var roleKPIs = await roleKPIsService.GetRoleKPIsAsync(cancellationToken);

            if (roleKPIs == null || roleKPIs.Count == 0)
            {
                return NotFound("No Role KPIs found.");
            }

            return Ok(roleKPIs);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get Role KPIs due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("{roleId}/{kpiId}")]
    [HasPermission(UserPermission.ManageRoles)]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(typeof(RoleKPIViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRoleKPIById(int roleId, int kpiId, CancellationToken cancellationToken)
    {
        try
        {
            var roleKPI = await roleKPIsService.GetRoleKPIByIdAsync(roleId, kpiId, cancellationToken);

            if (roleKPI == null)
            {
                return NotFound($"No Role KPI with RoleId={roleId} and KpiId={kpiId} found.");
            }

            return Ok(roleKPI);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get Role KPI due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost]
    [HasPermission(UserPermission.ManageRoles)]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(typeof(RoleKPIViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateRoleKPI([FromBody] AddUpdateRoleKPIRequest addUpdateRoleKPIRequest, CancellationToken cancellationToken)
    {
        if (addUpdateRoleKPIRequest == null)
        {
            return BadRequest("Invalid Role KPI data.");
        }

        try
        {
            addUpdateRoleKPIRequest.Validate();
            var roleKPI = await roleKPIsService.CreateRoleKPIAsync(addUpdateRoleKPIRequest, cancellationToken);
            return CreatedAtAction(nameof(GetRoleKPIById), new { roleId = roleKPI.RoleId, kpiId = roleKPI.KpiId }, roleKPI);
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Failed to create Role KPI due to validation operation");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to create Role KPI due to an invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Role KPI due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("{roleId}/{kpiId}")]
    [HasPermission(UserPermission.ManageRoles)]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(typeof(RoleKPIViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRoleKPI(int roleId, int kpiId, [FromBody] AddUpdateRoleKPIRequest addUpdateRoleKPIRequest, CancellationToken cancellationToken)
    {
        if (addUpdateRoleKPIRequest == null)
        {
            return BadRequest("Invalid Role KPI data.");
        }

        try
        {
            addUpdateRoleKPIRequest.Validate();
            var roleKPI = await roleKPIsService.UpdateRoleKPIAsync(roleId, kpiId, addUpdateRoleKPIRequest, cancellationToken);
            return Ok(roleKPI);
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Failed to update Role KPI due to validation operation");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to update Role KPI due to an invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update Role KPI due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpDelete("{roleId}/{kpiId}")]
    [HasPermission(UserPermission.ManageRoles)]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteRoleKPI(int roleId, int kpiId, CancellationToken cancellationToken)
    {
        try
        {
            await roleKPIsService.DeleteRoleKPIAsync(roleId, kpiId, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to delete Role KPI due to an invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete Role KPI due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}
