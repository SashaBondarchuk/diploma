using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllReports()
    {
        // Simulate getting all reports from a database
        var reports = new List<string> { "Report1", "Report2", "Report3" };
        return Ok(reports);
    }

    [HttpGet("{id}")]
    public IActionResult GetReportById(int id)
    {
        // Simulate getting a specific report by ID
        if (id <= 0)
        {
            return BadRequest("Invalid report ID.");
        }

        var report = $"Report{id}";
        return Ok(report);
    }

    [HttpGet("mine")]
    public IActionResult GetMyReports()
    {
        // Simulate getting reports for the current user
        var myReports = new List<string> { "MyReport1", "MyReport2" };
        return Ok(myReports);
    }
}
