using Employee.Performance.Evaluator.Core.Entities.Abstractions;

namespace Employee.Performance.Evaluator.Core.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTimeOffset BirthDate { get; set; }
    public DateTimeOffset HireDate { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public byte[]? Avatar { get; set; } = [];

    public Team? Team { get; set; }
    public User? User { get; set; }
    public ICollection<Recommendation>? Recommendations { get; set; }
    public ICollection<EvaluationSession>? EvaluationSessions { get; set; }
}
