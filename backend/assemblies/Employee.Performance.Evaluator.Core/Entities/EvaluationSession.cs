using Employee.Performance.Evaluator.Core.Entities.Abstractions;

namespace Employee.Performance.Evaluator.Core.Entities;

public class EvaluationSession : BaseEntity
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset? EvaluationFinishedDate { get; set; }
    public int EmployeeId { get; set; }
    public int? ClassId { get; set; }
    public decimal? WeightedScore { get; set; }
    public byte[]? ReportFile { get; set; } = [];

    public Employee? Employee { get; set; }
    public EmployeeClass? Class { get; set; }
    public ICollection<Evaluation> Evaluations { get; set; } = [];
}
