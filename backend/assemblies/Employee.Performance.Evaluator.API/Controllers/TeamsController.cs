using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Teams;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamsController(
    ILogger<TeamsController> logger,
    ITeamsService teamsService) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.Base)]
    [ProducesResponseType(typeof(List<TeamViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllTeams(CancellationToken cancellationToken)
    {
        try
        {
            var teams = await teamsService.GetAllTeamsAsync(cancellationToken);

            if (teams == null || teams.Count == 0)
            {
                return NotFound("No teams found.");
        }

            return Ok(teams);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get employees due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("{id}")]
    [HasPermission(UserPermission.Base)]
    [ProducesResponseType(typeof(TeamViewModelWithEmployees), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTeamById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var team = await teamsService.GetByIdAsync(id, cancellationToken);

            if (team == null)
            {
                return NotFound($"No team with Id={id} found.");
        }

            return Ok(team);
    }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get team due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost]
    [HasPermission(UserPermission.ManageTeams)]
    [ProducesResponseType(typeof(TeamViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTeam([FromBody] AddUpdateTeamRequest addUpdateTeamRequest, CancellationToken cancellationToken)
    {
        if (addUpdateTeamRequest == null)
        {
            return BadRequest("Team data is required.");
        }

        try
        {
            var createdTeam = await teamsService.CreateTeamAsync(addUpdateTeamRequest, cancellationToken);
            return Ok(createdTeam);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to create team");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create team due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("{id}")]
    [HasPermission(UserPermission.ManageTeams)]
    [ProducesResponseType(typeof(TeamViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTeam(int id, [FromBody] AddUpdateTeamRequest addUpdateTeamRequest, CancellationToken cancellationToken)
    {
        if (addUpdateTeamRequest == null)
        {
            return BadRequest("Team data is required.");
        }

        try
        {
            var updatedTeam = await teamsService.UpdateTeamAsync(id, addUpdateTeamRequest, cancellationToken);
            return Ok(updatedTeam);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to update team");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update team due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpDelete("{id}")]
    [HasPermission(UserPermission.ManageTeams)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTeam(int id, CancellationToken cancellationToken)
    {
        try
        {
            await teamsService.DeleteTeamAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
    {
            logger.LogError(ex, "Failed to delete team");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete team due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }

        return NoContent();
    }
}
