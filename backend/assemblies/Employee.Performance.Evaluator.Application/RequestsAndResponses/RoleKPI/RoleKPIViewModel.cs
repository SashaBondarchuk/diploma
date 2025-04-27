using RoleKPIEntity = Employee.Performance.Evaluator.Core.Entities.RoleKPI;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.RoleKPI;

public class RoleKPIViewModel
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;

    public int KpiId { get; set; }
    public string KpiName { get; set; } = string.Empty;

    public decimal Weight { get; set; }
    public int MinScore { get; set; }
    public int MaxScore { get; set; }
    public bool IsAllowedToEvaluateExceptLead { get; set; }
    public string ScoreRangeDescription { get; set; } = string.Empty;

    public static RoleKPIViewModel MapFromDbModel(RoleKPIEntity roleKPI)
    {
        return new RoleKPIViewModel
        {
            RoleId = roleKPI.RoleId,
            RoleName = roleKPI.Role?.RoleName ?? string.Empty,
            KpiId = roleKPI.KpiId,
            KpiName = roleKPI.KpiMetric?.Name ?? string.Empty,
            Weight = roleKPI.Weight,
            MinScore = roleKPI.MinScore,
            MaxScore = roleKPI.MaxScore,
            IsAllowedToEvaluateExceptLead = roleKPI.IsAllowedToEvaluateExceptLead,
            ScoreRangeDescription = roleKPI.ScoreRangeDescription
        };
    }
}
