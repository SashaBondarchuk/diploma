using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.EvaluationSessions;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EvaluationSessionsController(
    ILogger<EvaluationSessionsController> logger,
    IUserGetter userGetter,
    IEmployeeService employeeService,
    IEvaluationSessionsService evaluationSessionsService) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.ViewAllEvaluations)]
    [ProducesResponseType(typeof(List<EvaluationSessionViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? employeeId,
        [FromQuery] bool? isFinished,
        CancellationToken cancellationToken)
    {
        try
        {
            var evaluationSessions = await evaluationSessionsService.GetAllWithDetailsAsync(employeeId, isFinished, cancellationToken);

            if (evaluationSessions == null || evaluationSessions.Count == 0)
            {
                return NoContent();
            }

            return Ok(evaluationSessions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get evaluation sessions due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet("{id}")]
    [HasPermission(UserPermission.ViewAllEvaluations)]
    [ProducesResponseType(typeof(EvaluationSessionViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var evaluationSession = await evaluationSessionsService.GetByIdWithDetailsAsync(id, cancellationToken);

            if (evaluationSession == null)
            {
                return NotFound($"No evaluation session with Id={id} found.");
            }

            return Ok(evaluationSession);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get evaluation session due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    /// <summary>
    /// Get available sessions for current employee's team members' sessions)
    /// </summary>
    [HttpGet("pending-to-be-evaluated")]
    [HasPermission(UserPermission.EvaluateTeamMembers)]
    [ProducesResponseType(typeof(List<EvaluationSessionViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAvailableSessionsForCurrentEmployeeTeamMembers(CancellationToken cancellationToken)
    {
        try
        {
            var currentUser = userGetter.GetCurrentUserOrThrow();
            var employee = await employeeService.GetByUserIdAsync(currentUser.Id, cancellationToken);

            var evaluationSessions = await evaluationSessionsService.GetOngoingEvaluationsForEmployeeAsync(
                employee!.Id, currentUser, cancellationToken);

            if (evaluationSessions == null || evaluationSessions.Count == 0)
            {
                return NoContent();
            }

            return Ok(evaluationSessions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get available sessions for current employee team members due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    [HasPermission(UserPermission.ManageEvaluations)]
    [ProducesResponseType(typeof(EvaluationSessionViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] AddEvaluationSessionRequest addEvaluationSessionRequest, CancellationToken cancellationToken)
    {
        if (addEvaluationSessionRequest == null)
        {
            return BadRequest("Request body cannot be null.");
        }

        try
        {
            var evaluationSession = await evaluationSessionsService.CreateEvaluationSessionAsync(addEvaluationSessionRequest, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = evaluationSession.Id }, evaluationSession);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to create evaluation session due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create evaluation session due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost("end/{id}")]
    [HasPermission(UserPermission.ManageEvaluations)]
    [ProducesResponseType(typeof(EvaluationSessionViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> End(int id, CancellationToken cancellationToken)
    {
        try
        {
            var evaluationSession = await evaluationSessionsService.EndEvaluationSessionAsync(id, cancellationToken);
            return Ok(evaluationSession);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to end evaluation session due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to end evaluation session due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost("generate-report/{id}")]
    [HasPermission(UserPermission.ViewAllEvaluations)]
    [ProducesResponseType(typeof(EvaluationSessionViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateReport(int id, CancellationToken cancellationToken)
    {
        try
        {
            var report = await evaluationSessionsService.GenerateReportForSessionAsync(id, cancellationToken);
            return Ok(report);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to generate report for evaluation session due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to generate report for evaluation session due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("report/{id}")]
    [HasPermission(UserPermission.ViewAllEvaluations)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetReport(int id, CancellationToken cancellationToken)
    {
        try
        {
            var (reportBytes, filename) = await evaluationSessionsService.GetReportAsync(id, cancellationToken);

            if (reportBytes == null || reportBytes.Length == 0)
            {
                return BadRequest("Report was not generated.");
            }

            return File(
                fileContents: reportBytes,
                contentType: "application/pdf",
                fileDownloadName: filename
            );
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to get report for evaluation session due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get report for evaluation session due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [HasPermission(UserPermission.ManageEvaluations)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await evaluationSessionsService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to delete evaluation session due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete evaluation session due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}
