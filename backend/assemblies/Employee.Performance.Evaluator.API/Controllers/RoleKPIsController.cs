using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleKPIsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllRoleKPIs()
    {
        // Simulate getting all role KPIs from a database
        var roleKPIs = new List<string> { "RoleKPI1", "RoleKPI2", "RoleKPI3" };
        return Ok(roleKPIs);
    }

    [HttpPost]
    public IActionResult CreateRoleKPI([FromBody] string roleKPI)
    {
        // Simulate creating a new role KPI
        if (string.IsNullOrEmpty(roleKPI))
        {
            return BadRequest("Role KPI is required.");
        }

        return CreatedAtAction(nameof(GetAllRoleKPIs), new { Name = roleKPI });
    }

    [HttpPut("{roleId}/{kpiId}")]
    public IActionResult UpdateRoleKPI(int roleId, int kpiId, [FromBody] string roleKPI)
    {
        // Simulate updating a role KPI
        if (roleId <= 0 || kpiId <= 0 || string.IsNullOrEmpty(roleKPI))
        {
            return BadRequest("Invalid role ID, KPI ID or name.");
        }

        return Ok(new { RoleId = roleId, KpiId = kpiId, Name = roleKPI });
    }

    [HttpDelete("{roleId}/{kpiId}")]
    public IActionResult DeleteRoleKPI(int roleId, int kpiId)
    {
        // Simulate deleting a role KPI
        if (roleId <= 0 || kpiId <= 0)
        {
            return BadRequest("Invalid role ID or KPI ID.");
        }

        return NoContent();
    }
}
