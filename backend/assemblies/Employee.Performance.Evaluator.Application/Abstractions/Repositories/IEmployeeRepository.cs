using EmployeeEntity = Employee.Performance.Evaluator.Core.Entities.Employee;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IEmployeeRepository : IRepository<EmployeeEntity>
{
    Task<EmployeeEntity?> GetByIdWithDetailsAsync(int id);

    Task<IEnumerable<EmployeeEntity>> GetAllWithDetailsAsync();

    Task<EmployeeEntity?> GetByUserIdWithDetailsAsync(int userId);
}