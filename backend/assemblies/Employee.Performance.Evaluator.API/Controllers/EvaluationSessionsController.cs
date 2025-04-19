using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EvaluationSessionsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateEvaluationSession([FromBody] string sessionName)
    {
        if (string.IsNullOrEmpty(sessionName))
        {
            return BadRequest("Session name is required.");
        }

        // Simulate creating a new evaluation session
        return CreatedAtAction(nameof(CreateEvaluationSession), new { id = 1, Name = sessionName });
    }

    [HttpGet("{id}")]
    public IActionResult GetEvaluationSession(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid session ID.");
        }

        // Simulate getting an evaluation session
        var session = new { Id = id, Name = "Session " + id };
        return Ok(session);
    }

    [HttpGet("employee/{employeeId}")]
    public IActionResult GetEmployeeEvaluationHistory(int employeeId)
    {
        if (employeeId <= 0)
        {
            return BadRequest("Invalid employee ID.");
        }

        // Simulate getting evaluation history for an employee
        var history = new List<string>
        {
            "Evaluation 1 for Employee " + employeeId,
            "Evaluation 2 for Employee " + employeeId
        };

        return Ok(history);
    }

    [HttpGet("mine")]
    public IActionResult GetMyEvaluationSessions()
    {
        // Simulate getting evaluation sessions for the current user
        var sessions = new List<string>
        {
            "My Evaluation Session 1",
            "My Evaluation Session 2"
        };

        return Ok(sessions);
    }
}
