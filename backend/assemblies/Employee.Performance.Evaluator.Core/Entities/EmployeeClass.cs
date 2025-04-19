using Employee.Performance.Evaluator.Core.Entities.Abstractions;

namespace Employee.Performance.Evaluator.Core.Entities;

public class EmployeeClass : BaseEntity
{
    public string ClassName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] RecommendedActions { get; set; } = [];
    public decimal MinScore { get; set; }
    public decimal MaxScore { get; set; }
}
