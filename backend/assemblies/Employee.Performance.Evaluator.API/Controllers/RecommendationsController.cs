using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Recommendations;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecommendationsController(
    ILogger<RecommendationsController> logger,
    IRecommendationsService recommendationsService) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.CreateRecommendations)]
    [ProducesResponseType(typeof(List<RecommendationViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllRecommendations(CancellationToken cancellationToken)
    {
        try
        {
            var recommendations = await recommendationsService.GetAllRecommendationsAsync(cancellationToken);

            if (recommendations == null || !recommendations.Any())
            {
                return NoContent();
            }

            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get recommendations due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("{id}")]
    [HasPermission(UserPermission.Base)]
    [ProducesResponseType(typeof(RecommendationViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecommendationById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var recommendation = await recommendationsService.GetRecommendationByIdAsync(id, cancellationToken);

            if (recommendation == null)
            {
                return NotFound($"No recommendation with Id={id} found.");
            }

            return Ok(recommendation);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogError(ex, "Unauthorized access to recommendation with Id={Id}", id);
            return StatusCode((int)HttpStatusCode.Forbidden, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get recommendation due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("mine")]
    [HasPermission(UserPermission.Base)]
    [ProducesResponseType(typeof(List<RecommendationPartialViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecommendationsForCurrentEmployee(CancellationToken cancellationToken)
    {
        try
        {
            var recommendations = await recommendationsService.GetRecommendationsForCurrentEmployeeAsync(cancellationToken);

            if (recommendations == null || !recommendations.Any())
            {
                return NoContent();
            }

            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get recommendations for current employee due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost]
    [HasPermission(UserPermission.CreateRecommendations)]
    [ProducesResponseType(typeof(RecommendationViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddRecommendation([FromBody] AddRecommendationRequest addRecommendationRequest, CancellationToken cancellationToken)
    {
        if (addRecommendationRequest == null)
        {
            return BadRequest("AddRecommendationRequest cannot be null.");
        }

        try
        {
            var recommendation = await recommendationsService.AddRecommendationAsync(addRecommendationRequest, cancellationToken);

            return CreatedAtAction(nameof(GetRecommendationById), new { id = recommendation.Id }, recommendation);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to add recommendation due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add recommendation due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("{id}")]
    [HasPermission(UserPermission.CreateRecommendations)]
    [ProducesResponseType(typeof(RecommendationViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRecommendation(int id, [FromBody] AddRecommendationRequest updateRecommendationRequest, CancellationToken cancellationToken)
    {
        if (updateRecommendationRequest == null)
        {
            return BadRequest("UpdateRecommendationRequest cannot be null.");
        }

        try
        {
            var recommendation = await recommendationsService.UpdateRecommendationAsync(id, updateRecommendationRequest, cancellationToken);
            return Ok(recommendation);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to update recommendation due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update recommendation due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}
