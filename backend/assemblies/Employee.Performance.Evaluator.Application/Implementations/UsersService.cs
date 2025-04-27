using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Users;

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
}
