namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IUserIdSetter
{
    Task SetUserIdAsync(int userId, CancellationToken cancellationToken);
}
