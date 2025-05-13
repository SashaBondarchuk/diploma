using Employee.Performance.Evaluator.Application.RequestsAndResponses.Recommendations;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IRecommendationsService
{
    Task<IEnumerable<RecommendationViewModel>> GetAllRecommendationsAsync(CancellationToken cancellationToken);

    Task<RecommendationViewModel?> GetRecommendationByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<RecommendationPartialViewModel>> GetRecommendationsForCurrentEmployeeAsync(CancellationToken cancellationToken);

    Task<RecommendationViewModel> AddRecommendationAsync(AddRecommendationRequest addRecommendationRequest, CancellationToken cancellationToken);

    Task<RecommendationViewModel> UpdateRecommendationAsync(int id, AddRecommendationRequest updateRecommendationRequest, CancellationToken cancellationToken);
}
