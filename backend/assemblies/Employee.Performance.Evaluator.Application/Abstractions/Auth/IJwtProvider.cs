using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions.Auth;

public interface IJwtProvider
{
    Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken);
}
