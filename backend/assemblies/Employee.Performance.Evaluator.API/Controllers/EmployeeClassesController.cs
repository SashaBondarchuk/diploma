using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EmployeeClassesController(
    ILogger<EmployeeClassesController> logger,
    IEmployeeClassesService employeeClassesService) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.ViewAllEvaluations)]
    [ProducesResponseType(typeof(List<EmployeeClass>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var employeeClasses = await employeeClassesService.GetAllAsync(cancellationToken);

            if (employeeClasses == null || !employeeClasses.Any())
            {
                return NoContent();
            }

            return Ok(employeeClasses);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get employee classes due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    [HttpGet("{id}")]
    [HasPermission(UserPermission.ViewAllEvaluations)]
    [ProducesResponseType(typeof(EmployeeClass), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var employeeClass = await employeeClassesService.GetByIdAsync(id, cancellationToken);

            if (employeeClass == null)
            {
                return NotFound($"No employee class with Id={id} found.");
            }

            return Ok(employeeClass);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get employee class due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
