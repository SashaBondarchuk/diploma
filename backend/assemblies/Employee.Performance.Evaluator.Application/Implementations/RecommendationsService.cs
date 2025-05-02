using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.Recommendations;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class RecommendationsService(
    IEmployeesRepository employeesRepository,
    IUserGetter userGetter,
    IRecomendationsRepository recomendationsRepository) : IRecommendationsService
{
    public async Task<IEnumerable<RecommendationViewModel>> GetAllRecommendationsAsync(CancellationToken cancellationToken)
    {
        var recommendations = await recomendationsRepository.GetAllWithEmployeeAsync(cancellationToken);

        return recommendations.Select(RecommendationViewModel.MapFromDbModel);
    }

    public async Task<IEnumerable<RecommendationPartialViewModel>> GetRecommendationsForCurrentEmployeeAsync(CancellationToken cancellationToken)
    {
        var currentUser = userGetter.GetCurrentUserOrThrow();
        var employee = await employeesRepository.GetByUserIdAsync(currentUser.Id, cancellationToken);

        var recommendations = await recomendationsRepository.GetAllByEmployeeIdAsync(employee!.Id, cancellationToken);
        return recommendations.Select(RecommendationPartialViewModel.MapFromDbModel);
    }

    public async Task<RecommendationViewModel?> GetRecommendationByIdAsync(int id, CancellationToken cancellationToken)
    {
        var recommendation = await recomendationsRepository.GetByIdAsync(id, cancellationToken);

        return recommendation != null ? RecommendationViewModel.MapFromDbModel(recommendation) : null;
    }

    public async Task<RecommendationViewModel> AddRecommendationAsync(AddRecommendationRequest addRecommendationRequest, CancellationToken cancellationToken)
    {
        var employeeExists = await employeesRepository.ExistsAsync(addRecommendationRequest.EmployeeId, cancellationToken);
        if (!employeeExists)
        {
            throw new InvalidOperationException($"Employee with Id={addRecommendationRequest.EmployeeId} not found.");
        }

        var recommendation = new Recommendation
        {
            EmployeeId = addRecommendationRequest.EmployeeId,
            RecommendationText = addRecommendationRequest.RecommendationText,
            IsVisibleToEmployee = addRecommendationRequest.IsVisibleToEmployee,
            CreatedAt = DateTimeOffset.Now,
        };

        await recomendationsRepository.AddAsync(recommendation, cancellationToken);
        await recomendationsRepository.SaveChangesAsync(cancellationToken);

        var addedRecommendation = await recomendationsRepository.GetByIdWithDetailsAsync(recommendation.Id, cancellationToken);
        return RecommendationViewModel.MapFromDbModel(addedRecommendation!);
    }
}
