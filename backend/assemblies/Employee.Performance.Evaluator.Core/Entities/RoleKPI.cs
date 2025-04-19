namespace Employee.Performance.Evaluator.Core.Entities;

public class RoleKPI
{
    public int RoleId { get; set; }
    public int KpiId { get; set; }
    public decimal Weight { get; set; }
    public int MinScore { get; set; }
    public int MaxScore { get; set; }
    public bool IsAllowedToEvaluateExceptLead { get; set; }
    public string ScoreRangeDescription { get; set; } = string.Empty;

    public Role? Role { get; set; }
    public KPIMetric? KpiMetric { get; set; }
}
