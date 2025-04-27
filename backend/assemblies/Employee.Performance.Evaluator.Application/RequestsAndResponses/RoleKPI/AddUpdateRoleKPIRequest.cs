using System.Text;
using Employee.Performance.Evaluator.Core.Exceptions;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.RoleKPI;

public class AddUpdateRoleKPIRequest : IValidateable
{
    public int RoleId { get; set; }
    public int KpiId { get; set; }
    public decimal Weight { get; set; }
    public int MinScore { get; set; }
    public int MaxScore { get; set; }
    public bool IsAllowedToEvaluateExceptLead { get; set; }
    public string ScoreRangeDescription { get; set; } = string.Empty;

    public void Validate()
    {
        var errors = new StringBuilder();

        if (MinScore < 0)
            errors.AppendLine("MinScore must be greater than or equal to 0.");
        if (MaxScore <= 0)
            errors.AppendLine("MaxScore must be greater than 0.");
        if (MaxScore > 100)
            errors.AppendLine("MaxScore must be less than or equal to 100.");
        if (MaxScore < MinScore)
            errors.AppendLine("MaxScore must be greater than or equal to MinScore.");
        if (Weight < 0 || Weight > 1)
            errors.AppendLine("Weight must be between 0 and 1.");
        if (string.IsNullOrWhiteSpace(ScoreRangeDescription))
            errors.AppendLine("ScoreRangeDescription cannot be null or empty.");

        if (errors.Length > 0)
            throw new ValidationException(errors.ToString().TrimEnd());
    }
}
