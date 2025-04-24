using Employee.Performance.Evaluator.Application.RequestsAndResponses.Auth;

namespace Employee.Performance.Evaluator.Application.Abstractions.Auth;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
}
