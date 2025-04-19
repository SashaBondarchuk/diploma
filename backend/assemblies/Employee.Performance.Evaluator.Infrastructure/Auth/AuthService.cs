using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Auth;
using Employee.Performance.Evaluator.Core.Entities;
using Employee.Performance.Evaluator.Core.Enums;

namespace Employee.Performance.Evaluator.Infrastructure.Auth;

public class AuthService(
    IUsersRepository usersRepository,
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var userExists = (await usersRepository.GetAllAsync(cancellationToken)).Any(u => u.Email == request.Username);
        if (userExists)
        {
            throw new InvalidOperationException("Username is taken by other user.");
        }

        var user = new User
        {
            Email = request.Username,
            RoleId = (int)UserRole.Unassigned,
            PasswordHash = passwordHasher.HashPassword(request.Password)
        };

        var addedUser = await usersRepository.AddAsync(user, cancellationToken);
        await usersRepository.SaveChangesAsync(cancellationToken);

        var token = await jwtProvider.GenerateTokenAsync(addedUser, cancellationToken);

        return new AuthResponse { Token = token };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = (await usersRepository.GetAllAsync(cancellationToken)).FirstOrDefault(u => u.Email == request.Username);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Username or password is incorrect.");
        }

        var isValidPassword = passwordHasher.VerifyPassword(user.PasswordHash, request.Password);
        if (!isValidPassword)
        {
            throw new UnauthorizedAccessException("Username or password is incorrect.");
        }

        var token = await jwtProvider.GenerateTokenAsync(user, cancellationToken);

        return new AuthResponse { Token = token };
    }
}
