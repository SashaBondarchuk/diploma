using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Core.Exceptions;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class UserStorage(IUsersRepository usersRepository) : IUserGetter, IUserIdSetter
{
    private int? _id;
    private User? _user;

    public int? CurrentUserId
    {
        get => _id;
    }

    public User? CurrentUser
    {
        get => _user;
    }

    public int GetCurrentUserIdOrThrow()
    {
        if (_id == null)
        {
            throw new InvalidTokenException(_id.ToString());
        }

        return (int)_id;
    }

    public User GetCurrentUserOrThrow()
    {
        return _user ?? throw new InvalidTokenException(_id.ToString());
    }

    public async Task SetUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        if (_id != userId)
        {
            _user = await GetCurrentUserEntityAsync(userId, cancellationToken);
            _id = _user?.Id ?? null;
        }
    }

    private async Task<User?> GetCurrentUserEntityAsync(int userId, CancellationToken cancellationToken)
    {
        return (await usersRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(u => u.Id == userId);
    }
}
