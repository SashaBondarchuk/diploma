using Employee.Performance.Evaluator.Application.RequestsAndResponses.Evaluations;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IEvaluationsService
{
    Task<List<EvaluationViewModel>> GetAllBySessionIdAsync(int sessionId, CancellationToken cancellationToken);

    Task CreateEvaluationAsync(AddEvaluationRequest addEvaluationRequest, CancellationToken cancellationToken);
}
