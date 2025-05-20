using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IEmployeeClassesService
{
    Task<IEnumerable<EmployeeClass>> GetAllAsync(CancellationToken cancellationToken);

    Task<EmployeeClass?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
