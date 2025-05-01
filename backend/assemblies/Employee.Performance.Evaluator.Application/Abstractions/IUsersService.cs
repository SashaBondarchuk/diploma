using Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IUsersService
{
    Task<List<UserPartialViewModel>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserPartialViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken);
    
    bool HasTeamLeadPermissions(User user);
}
