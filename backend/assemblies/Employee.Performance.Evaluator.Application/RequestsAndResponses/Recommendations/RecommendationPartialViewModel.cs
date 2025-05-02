using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.RequestsAndResponses.Recommendations;

public class RecommendationPartialViewModel
{
    public int Id { get; set; }
    public string RecommendationText { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public static RecommendationPartialViewModel MapFromDbModel(Recommendation recommendation)
    {
        return new RecommendationPartialViewModel
        {
            Id = recommendation.Id,
            RecommendationText = recommendation.RecommendationText,
            CreatedAt = recommendation.CreatedAt
        };
    }
}
