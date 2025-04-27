using Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IUsersService
{
    Task<List<UserPartialViewModel>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserPartialViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
