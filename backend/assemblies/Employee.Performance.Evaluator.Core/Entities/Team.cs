using Employee.Performance.Evaluator.Core.Entities.Abstractions;

namespace Employee.Performance.Evaluator.Core.Entities;

public class Team : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int? TeamLeadId { get; set; }

    public Employee? TeamLead { get; set; }
    public ICollection<Employee>? Employees { get; set; }
}
