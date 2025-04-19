using Employee.Performance.Evaluator.Core.Entities.Abstractions;

namespace Employee.Performance.Evaluator.Core.Entities;

public class KPIMetric : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
