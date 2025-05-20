using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class EmployeeClassesService(IEmployeeClassesRepository employeeClassesRepository) : IEmployeeClassesService
{
    public Task<EmployeeClass?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return employeeClassesRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<IEnumerable<EmployeeClass>> GetAllAsync(CancellationToken cancellationToken)
    {
        return employeeClassesRepository.GetAllAsync(cancellationToken);
    }
}
