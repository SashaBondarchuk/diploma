using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;
using Employee.Performance.Evaluator.Core.Enums;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class EmployeesService(
    IEmployeeRepository employeeRepository,
    IUsersRepository usersRepository) : IEmployeeService
{
    public async Task<List<EmployeeViewModel>> GetEmployeesAsync(CancellationToken cancellationToken)
    {
        var employees = await employeeRepository.GetAllWithDetailsAsync(cancellationToken);

        return [.. employees.Select(e => EmployeeViewModel.MapFromDbModel(e))];
    }

    public async Task<EmployeeViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(id, cancellationToken);

        return employee == null ? null : EmployeeViewModel.MapFromDbModel(employee);
    }

    public async Task<EmployeeViewModel?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken);

        return employee == null ? null : EmployeeViewModel.MapFromDbModel(employee);
    }

    public async Task<EmployeeViewModel> CreateAsync(AddUpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var employeeToCreate = AddUpdateEmployeeRequest.ToDbModel(request);
        employeeToCreate.HireDate = DateTimeOffset.UtcNow;

        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with Id={request.UserId} not found.");
        }

        var asiggnedEmployee = await employeeRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (asiggnedEmployee != null)
        {
            throw new InvalidOperationException($"User with Id={request.UserId} is already assigned to another employee.");
        }

        user.RoleId = request.RoleId;
        usersRepository.Update(user);

        var addedEmployee = await employeeRepository.AddAsync(employeeToCreate, cancellationToken);
        await employeeRepository.SaveChangesAsync(cancellationToken);

        var addedEmployeeWithDetails = await employeeRepository.GetByIdWithDetailsAsync(addedEmployee.Id, cancellationToken);

        return EmployeeViewModel.MapFromDbModel(addedEmployeeWithDetails!);
    }

    public async Task<EmployeeViewModel> UpdateAsync(int id, AddUpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var existingEmployee = await employeeRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (existingEmployee == null)
        {
            throw new InvalidOperationException($"Employee with Id={id} not found.");
        }
        if (existingEmployee.UserId != request.UserId)
        {
            throw new InvalidOperationException($"Cannot change the user of an existing employee.");
        }

        existingEmployee.FirstName = request.FirstName;
        existingEmployee.LastName = request.LastName;
        existingEmployee.PhoneNumber = request.PhoneNumber;
        existingEmployee.BirthDate = request.BirthDate;
        existingEmployee.TeamId = request.TeamId;
        existingEmployee.Avatar = request.Avatar;
        existingEmployee.User!.RoleId = request.RoleId;

        employeeRepository.Update(existingEmployee);
        await employeeRepository.SaveChangesAsync(cancellationToken);

        var updatedEmployee = await employeeRepository.GetByIdWithDetailsAsync(existingEmployee.Id, cancellationToken);

        return EmployeeViewModel.MapFromDbModel(updatedEmployee!);
    }

    public async Task DeactivateEmployeeAsync(int id, CancellationToken cancellationToken)
    {
        var existingEmployee = await employeeRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (existingEmployee == null)
        {
            throw new InvalidOperationException($"Employee with Id={id} not found.");
        }

        existingEmployee.User!.RoleId = (int)UserRole.Unassigned;
        employeeRepository.Update(existingEmployee);

        await employeeRepository.SaveChangesAsync(cancellationToken);
    }
}
