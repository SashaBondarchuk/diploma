using System.Net;
using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Core.Exceptions;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Performance.Evaluator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(
    ILogger<EmployeesController> logger,
    IEmployeeService employeeService,
    IUserGetter userGetter) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.Base)]
    [ProducesResponseType(typeof(List<EmployeeViewModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var employees = await employeeService.GetEmployeesAsync(cancellationToken);

            if (employees == null || employees.Count == 0)
            {
                return NotFound("No employees found.");
            }

            return Ok(employees);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get employees due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("{id}")]
    [HasPermission(UserPermission.Base)]
    [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var employee = await employeeService.GetByIdAsync(id, cancellationToken);

            if (employee == null)
            {
                return NotFound($"No employee with Id={id} found.");
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get employee with Id={Id} due to an unexpected error", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpGet("me")]
    [HasPermission(UserPermission.Base)]
    [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCurrentEmployee(CancellationToken cancellationToken)
    {
        try
        {
            var userId = userGetter.GetCurrentUserIdOrThrow();
            var employee = await employeeService.GetByUserIdAsync(userId, cancellationToken);

            if (employee == null)
            {
                return NotFound($"Employee with Id={userId} not found.");
            }

            return Ok(employee);
        }
        catch (InvalidTokenException ex)
        {
            logger.LogError(ex, "Failed to get current employee");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving current employee.");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPost]
    [HasPermission(UserPermission.ManageEmployees)]
    [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] AddUpdateEmployeeRequest employee, CancellationToken cancellationToken)
    {
        if (employee == null)
        {
            return BadRequest("Employee data is required.");
        }

        try
        {
            var createdEmployee = await employeeService.CreateAsync(employee, cancellationToken);
            return CreatedAtAction(nameof(Create), new { id = createdEmployee.Id }, createdEmployee);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to create employee");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create employee due to an unexpected error");
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("{id}")]
    [HasPermission(UserPermission.ManageEmployees)]
    [ProducesResponseType(typeof(EmployeeViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] AddUpdateEmployeeRequest employee, CancellationToken cancellationToken)
    {
        if (employee == null)
        {
            return BadRequest("Employee data is required.");
        }

        try
        {
            var updatedEmployee = await employeeService.UpdateAsync(id, employee, cancellationToken);
            return Ok(updatedEmployee);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to get employee with Id={Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update employee with Id={Id} due to an unexpected error", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    [HttpPut("deactivate/{id}")]
    [HasPermission(UserPermission.ManageEmployees)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Deactivate(int id, CancellationToken cancellationToken)
    {
        try
        {
            await employeeService.DeactivateEmployeeAsync(id, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Failed to deactivate employee with Id={Id} due to an invalid operation", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get employee with Id={Id} due to an unexpected error", id);
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}