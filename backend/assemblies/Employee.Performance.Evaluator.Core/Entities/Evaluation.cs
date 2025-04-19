using Employee.Performance.Evaluator.Core.Entities.Abstractions;

namespace Employee.Performance.Evaluator.Core.Entities;

public class Evaluation : BaseEntity
{
    public int Score { get; set; }
    public string Comment { get; set; } = string.Empty;
    public int EvaluationSessionId { get; set; }
    public int KpiId { get; set; }
    public int RoleId { get; set; }
    public int EvaluatorId { get; set; }

    public Employee? Evaluator { get; set; }
    public RoleKPI? RoleKpi { get; set; }
    public EvaluationSession? EvaluationSession { get; set; }
}
