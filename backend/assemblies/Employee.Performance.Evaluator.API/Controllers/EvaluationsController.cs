using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Evaluations;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EvaluationsController(
    ILogger<EvaluationsController> logger,
    IEvaluationsService evaluationsService) : ControllerBase
{
    [HttpGet("session/{sessionId}")]
    [HasPermission(UserPermission.ManageEvaluations)]
    [ProducesResponseType(typeof(IEnumerable<EvaluationViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllBySessionIdAsync(int sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var evaluations = await evaluationsService.GetAllBySessionIdAsync(sessionId, cancellationToken);

            if (evaluations == null || evaluations.Count == 0)
            {
                return NoContent();
            }

            return Ok(evaluations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting evaluations.");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost]
    [HasPermission(UserPermission.EvaluateTeamMembers)]
    [ProducesResponseType(typeof(EvaluationViewModel), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateEvaluationAsync(
        [FromBody] AddEvaluationRequest addEvaluationRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            await evaluationsService.CreateEvaluationAsync(addEvaluationRequest, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Validation error occurred while creating evaluation.");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating evaluation.");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}
