using Employee.Performance.Evaluator.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecommendationsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateRecommendation([FromBody] Recommendation recommendation)
    {
        if (recommendation == null)
        {
            return BadRequest("Recommendation data is required.");
        }

        // Simulate creating a new recommendation
        return CreatedAtAction(nameof(CreateRecommendation), new { id = recommendation.Id }, recommendation);
    }

    [HttpGet("employee/{employeeId}")]
    public IActionResult GetRecommendationsForEmployee(int employeeId)
    {
        // Simulate getting recommendations for an employee
        var recommendations = new List<Recommendation>
        {
            new Recommendation { Id = 1, EmployeeId = employeeId, RecommendationText = "Great job on the project!" },
            new Recommendation { Id = 2, EmployeeId = employeeId, RecommendationText = "Keep up the good work!" }
        };

        return Ok(recommendations);
    }

    [HttpGet("mine")]
    public IActionResult GetMyRecommendations()
    {
        // Simulate getting recommendations for the current user
        var recommendations = new List<Recommendation>
        {
            new Recommendation { Id = 1, EmployeeId = 1, RecommendationText = "Great job on the project!" },
            new Recommendation { Id = 2, EmployeeId = 1, RecommendationText = "Keep up the good work!" }
        };

        return Ok(recommendations);
    }
}
