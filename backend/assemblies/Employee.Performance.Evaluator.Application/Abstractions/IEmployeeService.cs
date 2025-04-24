using Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IEmployeeService
{
    Task<List<EmployeeViewModel>> GetEmployeesAsync(CancellationToken cancellationToken);

    Task<EmployeeViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<EmployeeViewModel?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);

    Task<EmployeeViewModel> CreateAsync(AddUpdateEmployeeRequest request, CancellationToken cancellationToken);

    Task<EmployeeViewModel> UpdateAsync(int id, AddUpdateEmployeeRequest request, CancellationToken cancellationToken);

    Task DeactivateEmployeeAsync(int id, CancellationToken cancellationToken);
}
