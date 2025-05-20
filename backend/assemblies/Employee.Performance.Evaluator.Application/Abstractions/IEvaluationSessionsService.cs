using Employee.Performance.Evaluator.Application.RequestsAndResponses.EvaluationSessions;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IEvaluationSessionsService
{
    Task<List<EvaluationSessionViewModel>> GetAllWithDetailsAsync(int? employeeId, bool? isFinished, CancellationToken cancellationToken);

    Task<List<EvaluationSessionViewModel>> GetOngoingEvaluationsForEmployeeAsync(int employeeId, User currentUser, CancellationToken cancellationToken);

    Task<EvaluationSessionViewModel?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken);

    Task<EvaluationSessionViewModel> CreateEvaluationSessionAsync(AddEvaluationSessionRequest addEvaluationSessionRequest, CancellationToken cancellationToken);

    Task<EvaluationSessionViewModel> EndEvaluationSessionAsync(int id, CancellationToken cancellationToken);

    Task<EvaluationSessionViewModel> GenerateReportForSessionAsync(int id, CancellationToken cancellationToken);

    Task<(byte[] content, string fileName)> GetReportAsync(int sessionId, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
