using Employee.Performance.Evaluator.Core.Entities.Abstractions;

namespace Employee.Performance.Evaluator.Core.Entities;

public class Role : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;

    public ICollection<Permission> Permissions { get; set; } = [];
}
