using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Repositories;

public interface IUsersRepository : IRepository<User>
{
    Task<HashSet<string>> GetPermissionsAsync(int userId, CancellationToken cancellationToken);
}
