using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;
using Employee.Performance.Evaluator.Core.Enums;
using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class EmployeesService(
    ITransactionService transactionService,
    IEmployeesRepository employeeRepository,
    ITeamsRepository teamsRepository,
    IRolesRepository rolesRepository,
    IUsersRepository usersRepository) : IEmployeeService
{
    public async Task<List<EmployeeViewModel>> GetEmployeesAsync(CancellationToken cancellationToken)
    {
        var employees = await employeeRepository.GetAllWithDetailsAsync(cancellationToken);

        return [.. employees.Select(e => EmployeeViewModel.MapFromDbModel(e))];
    }

    public async Task<EmployeeViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAndTeamMembersAsync(id, cancellationToken);

        return employee == null ? null : EmployeeViewModel.MapFromDbModel(employee);
    }

    public async Task<EmployeeViewModel?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken);

        return employee == null ? null : EmployeeViewModel.MapFromDbModel(employee);
    }

    public async Task<EmployeeViewModel> CreateAsync(AddUpdateEmployeeRequest employeeToCreate, CancellationToken cancellationToken)
    {
        var employeeToCreateEntity = AddUpdateEmployeeRequest.ToDbModel(employeeToCreate);
        employeeToCreateEntity.HireDate = DateTimeOffset.Now;

        var user = await usersRepository.GetByIdAsync(employeeToCreateEntity.UserId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with Id={employeeToCreateEntity.UserId} not found.");
        }

        var team = await teamsRepository.GetByIdAsync((int)employeeToCreateEntity.TeamId!, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException($"Team with Id={employeeToCreateEntity.TeamId} not found.");
        }

        var roleExists = await rolesRepository.ExistsAsync(employeeToCreate.RoleId, cancellationToken);
        if (!roleExists)
        {
            throw new InvalidOperationException($"Role with Id={employeeToCreate.RoleId} not found.");
        }

        var assignedEmployee = await employeeRepository.GetByUserIdAsync(employeeToCreateEntity.UserId, cancellationToken);
        if (assignedEmployee != null)
        {
            throw new InvalidOperationException($"User with Id={employeeToCreateEntity.UserId} is already assigned to another employee.");
        }

        EmployeeEntity addedEmployee = null!;
        await transactionService.ExecuteInTransactionAsync(async () =>
        {
            user.RoleId = employeeToCreate.RoleId;
            usersRepository.Update(user);
            await usersRepository.SaveChangesAsync(cancellationToken);

            addedEmployee = await employeeRepository.AddAsync(employeeToCreateEntity, cancellationToken);
            await employeeRepository.SaveChangesAsync(cancellationToken);

            if (employeeToCreate.IsTeamLead)
            {
                team.TeamLeadId = addedEmployee.Id;
                teamsRepository.Update(team);
                await teamsRepository.SaveChangesAsync(cancellationToken);
            }
        }, cancellationToken);

        var addedEmployeeWithDetails = await employeeRepository.GetByIdWithDetailsAndTeamMembersAsync(addedEmployee.Id, cancellationToken);

        return EmployeeViewModel.MapFromDbModel(addedEmployeeWithDetails!);
    }

    public async Task<EmployeeViewModel> UpdateAsync(int id, AddUpdateEmployeeRequest employeeToUpdate, CancellationToken cancellationToken)
    {
        var existingEmployee = await employeeRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (existingEmployee == null)
        {
            throw new InvalidOperationException($"Employee with Id={id} not found.");
        }

        if (existingEmployee.UserId != employeeToUpdate.UserId)
        {
            throw new InvalidOperationException($"Cannot change the user of an existing employee.");
        }

        var roleExists = await rolesRepository.ExistsAsync(employeeToUpdate.RoleId, cancellationToken);
        if (!roleExists)
        {
            throw new InvalidOperationException($"Role with Id={employeeToUpdate.RoleId} not found.");
        }

        existingEmployee.FirstName = employeeToUpdate.FirstName;
        existingEmployee.LastName = employeeToUpdate.LastName;
        existingEmployee.PhoneNumber = employeeToUpdate.PhoneNumber;
        existingEmployee.BirthDate = employeeToUpdate.BirthDate;
        existingEmployee.Avatar = string.IsNullOrEmpty(employeeToUpdate.Avatar)
            ? null
            : Convert.FromBase64String(
                employeeToUpdate.Avatar.Split(',').Last()
            );
        existingEmployee.User!.RoleId = employeeToUpdate.RoleId;

        await transactionService.ExecuteInTransactionAsync(async () =>
        {
            var currentTeam = existingEmployee.Team!;
            if (existingEmployee.TeamId != employeeToUpdate.TeamId)
            {
                var newEmployeeTeam = await teamsRepository.GetByIdAsync(employeeToUpdate.TeamId, cancellationToken);
                if (newEmployeeTeam == null)
                {
                    throw new InvalidOperationException($"Team with Id={employeeToUpdate.TeamId} not found.");
                }

                if (currentTeam!.TeamLeadId == existingEmployee.Id)
                {
                    currentTeam.TeamLeadId = null;
                }
                if (employeeToUpdate.IsTeamLead)
                {
                    newEmployeeTeam.TeamLeadId = existingEmployee.Id;
                }

                teamsRepository.Update(newEmployeeTeam);
                existingEmployee.TeamId = employeeToUpdate.TeamId;
            }
            else if (employeeToUpdate.IsTeamLead)
            {
                currentTeam.TeamLeadId = existingEmployee.Id;
            }

            teamsRepository.Update(currentTeam);
            await teamsRepository.SaveChangesAsync(cancellationToken);

            employeeRepository.Update(existingEmployee);
            await employeeRepository.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

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

        if (existingEmployee.User!.Role!.Id == (int)UserRole.Unassigned)
        {
            throw new InvalidOperationException($"Employee with Id={id} is already deactivated.");
        }

        await transactionService.ExecuteInTransactionAsync(async () =>
        {
            existingEmployee.User!.RoleId = (int)UserRole.Unassigned;
            employeeRepository.Update(existingEmployee);
            await employeeRepository.SaveChangesAsync(cancellationToken);

            var employeeTeam = existingEmployee.Team;
            if (employeeTeam!.TeamLeadId == existingEmployee.Id)
            {
                employeeTeam.TeamLeadId = null;
            }

            employeeTeam!.Employees!.Remove(existingEmployee);

            teamsRepository.Update(employeeTeam);
            await teamsRepository.SaveChangesAsync(cancellationToken);
        }, cancellationToken);
    }
}
