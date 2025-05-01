using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Core.Enums;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class UsersService(IUsersRepository usersRepository) : IUsersService
{
    public async Task<List<UserPartialViewModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await usersRepository.GetAllWithDetailsAsync(cancellationToken);

        return [.. users.Select(UserPartialViewModel.MapFromDbModel)];
    }

    public async Task<UserPartialViewModel?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdWithDetailsAsync(id, cancellationToken);

        return user == null ? null : UserPartialViewModel.MapFromDbModel(user);
    }

    public bool HasTeamLeadPermissions(User user)
    {
        if (user.Role == null)
        {
            throw new InvalidOperationException($"User with Id={user.Id} has no role included.");
        }

        var userPermissions = user.Role.Permissions;
        return userPermissions.Any(x => x.Id == (int)UserPermission.EvaluateTeamMembersLead);
    }
}
