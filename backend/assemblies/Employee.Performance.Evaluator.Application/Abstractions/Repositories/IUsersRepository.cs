using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IUsersRepository : IRepository<User>
{
    Task<List<User>> GetAllWithDetailsAsync(CancellationToken cancellationToken);

    Task<User?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken);

    Task<HashSet<string>> GetPermissionsAsync(int userId, CancellationToken cancellationToken);
}
