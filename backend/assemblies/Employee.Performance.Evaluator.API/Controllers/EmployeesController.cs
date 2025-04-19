using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Enums;
using Employee.Performance.Evaluator.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;
using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(
    ILogger<EmployeesController> logger,
    IEmployeeRepository employeeRepository,
    IUserGetter userGetter) : ControllerBase
{
    [HttpGet]
    [HasPermission(UserPermission.EditEmployee)]
    public async Task<IActionResult> GetAll()
    {
        var employees = await employeeRepository.GetAllWithDetailsAsync();

        if (employees == null || !employees.Any())
        {
            return NotFound("No employees found.");
        }

        return Ok(employees);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeEntity employee)
    {
        if (employee == null)
        {
            return BadRequest("Employee data is required.");
        }

        //await employeeRepository.AddAsync(employee);
        return CreatedAtAction(nameof(Create), new { id = employee.Id }, employee);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeEntity employee)
    {
        if (id != employee.Id)
        {
            return BadRequest("Employee ID mismatch.");
        }

        var existingEmployee = await employeeRepository.GetByIdWithDetailsAsync(id);
        if (existingEmployee == null)
        {
            return NotFound($"Employee with ID {id} not found.");
        }

        //await employeeRepository.UpdateAsync(employee);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingEmployee = await employeeRepository.GetByIdWithDetailsAsync(id);
        if (existingEmployee == null)
        {
            return NotFound($"Employee with ID {id} not found.");
        }

        //await employeeRepository.DeleteAsync(existingEmployee);
        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var userId = userGetter.GetCurrentUserIdOrThrow();
            var employee = await employeeRepository.GetByIdWithDetailsAsync(userId);

            if (employee == null)
            {
                return NotFound($"Employee with ID {userId} not found.");
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving current user.");
            return BadRequest(ex.Message);
        }
    }
}