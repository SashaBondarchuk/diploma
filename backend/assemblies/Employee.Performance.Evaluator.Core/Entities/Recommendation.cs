using Employee.Performance.Evaluator.Core.Entities.Abstractions;

namespace Employee.Performance.Evaluator.Core.Entities;

public class Recommendation : BaseEntity
{
    public int EmployeeId { get; set; }
    public string RecommendationText { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}
