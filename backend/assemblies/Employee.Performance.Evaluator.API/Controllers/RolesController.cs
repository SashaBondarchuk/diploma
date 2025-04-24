using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetRoles()
    {
        // Simulate getting roles from a database
        var roles = new List<string> { "Admin", "User", "Manager" };
        return Ok(roles);
    }

    [HttpPost]
    public IActionResult CreateRole([FromBody] string roleName)
    {
        // Simulate creating a new role
        if (string.IsNullOrEmpty(roleName))
        {
            return BadRequest("Role name is required.");
        }

        return CreatedAtAction(nameof(GetRoles), new { Name = roleName });
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRole(int id, [FromBody] string roleName)
    {
        // Simulate updating a role
        if (id <= 0 || string.IsNullOrEmpty(roleName))
        {
            return BadRequest("Invalid role ID or name.");
        }

        return Ok(new { Id = id, Name = roleName });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRole(int id)
    {
        // Simulate deleting a role
        if (id <= 0)
        {
            return BadRequest("Invalid role ID.");
        }

        return NoContent();
    }
}
