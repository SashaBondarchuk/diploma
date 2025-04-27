using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.KPIMetric;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KPIMetricsController(
    ILogger<KPIMetricsController> logger,
    IKPIMetricsService kPIMetricsService) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(typeof(List<KPIMetric>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllKPIMetrics(CancellationToken cancellationToken)
    {
        try
        {
            var kPIMetricsList = await kPIMetricsService.GetKPIMetricsAsync(cancellationToken);

            if (kPIMetricsList == null || kPIMetricsList.Count == 0)
            {
                return NotFound("No KPI metrics found.");
            }

            return Ok(kPIMetricsList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get KPI metrics due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("{id}")]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(typeof(KPIMetric), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetKPIMetricById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var kPIMetric = await kPIMetricsService.GetByIdAsync(id, cancellationToken);

            if (kPIMetric == null)
            {
                return NotFound($"No KPI metric with Id={id} found.");
            }

            return Ok(kPIMetric);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get KPI metric due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(typeof(KPIMetric), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateKPIMetric([FromBody] AddUpdateKPIMetricRequest addUpdateKPIMetricRequest, CancellationToken cancellationToken)
    {
        if (addUpdateKPIMetricRequest == null)
        {
            return BadRequest("The KPI metric name is required.");
        }

        try
        {
            var kPIMetric = await kPIMetricsService.CreateKPIMetricAsync(addUpdateKPIMetricRequest, cancellationToken);
            return CreatedAtAction(nameof(GetKPIMetricById), new { id = kPIMetric.Id }, kPIMetric);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to create KPI metric due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create KPI metric due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("{id}")]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(typeof(KPIMetric), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateKPIMetric(int id, [FromBody] AddUpdateKPIMetricRequest addUpdateKPIMetricRequest, CancellationToken cancellationToken)
    {
        if (addUpdateKPIMetricRequest == null)
        {
            return BadRequest("The KPI metric name is required.");
        }

        try
        {
            var kPIMetric = await kPIMetricsService.UpdateKPIMetricAsync(id, addUpdateKPIMetricRequest, cancellationToken);
            return Ok(kPIMetric);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to update KPI metric due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update KPI metric due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpDelete("{id}")]
    [HasPermission(UserPermission.ManageKpis)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteKPIMetric(int id, CancellationToken cancellationToken)
    {
        try
        {
            await kPIMetricsService.DeleteKPIMetricAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to delete KPI metric due to invalid operation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete KPI metric due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}
