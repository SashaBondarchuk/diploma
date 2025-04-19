using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamsController(ITeamsRepository teamsRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllTeams(CancellationToken cancellationToken)
    {
        try
        {
            var teams = await teamsRepository.GetAllAsync(cancellationToken);

        }
        catch (Exception)
        {

            throw;
        }
    }

    [HttpPost]
    public IActionResult CreateTeam([FromBody] string teamName)
    {
        // Simulate creating a new team
        if (string.IsNullOrEmpty(teamName))
        {
            return BadRequest("Team name is required.");
        }

        return CreatedAtAction(nameof(GetAllTeams), new { Name = teamName });
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTeam(int id, [FromBody] string teamName)
    {
        // Simulate updating a team
        if (id <= 0 || string.IsNullOrEmpty(teamName))
        {
            return BadRequest("Invalid team ID or name.");
        }

        return Ok(new { Id = id, Name = teamName });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTeam(int id)
    {
        // Simulate deleting a team
        if (id <= 0)
        {
            return BadRequest("Invalid team ID.");
        }

        return NoContent();
    }
}
