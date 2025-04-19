using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KPIMetricsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllKPI()
    {
        // Simulate getting all KPIs from a database
        var kpis = new List<string> { "KPI1", "KPI2", "KPI3" };
        return Ok(kpis);
    }

    [HttpPost]
    public IActionResult CreateKPI([FromBody] string kpiName)
    {
        // Simulate creating a new KPI
        if (string.IsNullOrEmpty(kpiName))
        {
            return BadRequest("KPI name is required.");
        }

        return CreatedAtAction(nameof(GetAllKPI), new { Name = kpiName });
    }

    [HttpPut("{id}")]
    public IActionResult UpdateKPI(int id, [FromBody] string kpiName)
    {
        // Simulate updating a KPI
        if (id <= 0 || string.IsNullOrEmpty(kpiName))
        {
            return BadRequest("Invalid KPI ID or name.");
        }

        return Ok(new { Id = id, Name = kpiName });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteKPI(int id)
    {
        // Simulate deleting a KPI
        if (id <= 0)
        {
            return BadRequest("Invalid KPI ID.");
        }

        return NoContent();
    }
}
