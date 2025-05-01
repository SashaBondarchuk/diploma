using Employee.Performance.Evaluator.Application.RequestsAndResponses.Employees;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.RoleKPI;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Evaluations;

public class EvaluationViewModel
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int EvaluationSessionId { get; set; }
    public int KpiId { get; set; }
    public int RoleId { get; set; }
    public int EvaluatorId { get; set; }

    public EmployeePartialViewModel Evaluator { get; set; } = new();
    public RoleKPIViewModel RoleKpi { get; set; } = new();

    public static EvaluationViewModel MapFromDbModel(Evaluation evaluation)
    {
        return new EvaluationViewModel
        {
            Id = evaluation.Id,
            Score = evaluation.Score,
            Comment = evaluation.Comment,
            EvaluationSessionId = evaluation.EvaluationSessionId,
            KpiId = evaluation.KpiId,
            RoleId = evaluation.RoleId,
            EvaluatorId = evaluation.EvaluatorId,
            Evaluator = EmployeePartialViewModel.MapFromDbModel(evaluation.Evaluator!),
            RoleKpi = RoleKPIViewModel.MapFromDbModel(evaluation.RoleKpi!)
        };
    }
}
