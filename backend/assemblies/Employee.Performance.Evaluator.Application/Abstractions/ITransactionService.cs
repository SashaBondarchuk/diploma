namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface ITransactionService
{
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken);
}
