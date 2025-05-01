using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IEmployeesRepository : IRepository<EmployeeEntity>
{
    Task<EmployeeEntity?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<EmployeeEntity>> GetAllWithDetailsAsync(CancellationToken cancellationToken);

    Task<EmployeeEntity?> GetByUserIdWithDetailsAsync(int userId, CancellationToken cancellationToken);

    Task<EmployeeEntity?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
    
    Task<EmployeeEntity?> GetByIdWithDetailsAndTeamMembersAsync(int id, CancellationToken cancellationToken);
}