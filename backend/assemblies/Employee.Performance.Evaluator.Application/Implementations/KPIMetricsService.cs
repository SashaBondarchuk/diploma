using Employee.Performance.Evaluator.Application.Abstractions;
using Employee.Performance.Evaluator.Application.Abstractions.Repositories;
using Employee.Performance.Evaluator.Application.RequestsAndResponses.KPIMetric;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Implementations;

public class KPIMetricsService(IKPIMetricsRepository kPIMetricsRepository) : IKPIMetricsService
{
    public async Task<List<KPIMetric>> GetKPIMetricsAsync(CancellationToken cancellationToken)
    {
        var kPIMetrics = await kPIMetricsRepository.GetAllAsync(cancellationToken);

        return [.. kPIMetrics];
    }

    public async Task<KPIMetric?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var kPIMetric = await kPIMetricsRepository.GetByIdAsync(id, cancellationToken);

        return kPIMetric;
    }

    public async Task<KPIMetric> CreateKPIMetricAsync(AddUpdateKPIMetricRequest addUpdateKPIMetricRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(addUpdateKPIMetricRequest.Name))
        {
            throw new InvalidOperationException("The KPI metric name is required.");
        }

        var metrics = await kPIMetricsRepository.GetAllAsync(cancellationToken);
        if (metrics.Any(m => m.Name == addUpdateKPIMetricRequest.Name))
        {
            throw new InvalidOperationException($"The KPI metric name '{addUpdateKPIMetricRequest.Name}' is already in use.");
        }

        var kPIMetricToCreate = new KPIMetric()
        {
            Name = addUpdateKPIMetricRequest.Name,
        };

        var addedKPIMetric = await kPIMetricsRepository.AddAsync(kPIMetricToCreate, cancellationToken);
        await kPIMetricsRepository.SaveChangesAsync(cancellationToken);

        return addedKPIMetric;
    }

    public async Task<KPIMetric> UpdateKPIMetricAsync(int id, AddUpdateKPIMetricRequest addUpdateKPIMetricRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(addUpdateKPIMetricRequest.Name))
        {
            throw new InvalidOperationException("The KPI metric name is required.");
        }

        var kPIMetricToUpdate = await kPIMetricsRepository.GetByIdAsync(id, cancellationToken);
        if (kPIMetricToUpdate == null)
        {
            throw new InvalidOperationException($"No KPI metric with Id={id} found.");
        }

        if (kPIMetricToUpdate.Name == addUpdateKPIMetricRequest.Name)
        {
            throw new InvalidOperationException($"The KPI metric name is already in use.");
        }

        kPIMetricToUpdate.Name = addUpdateKPIMetricRequest.Name;
        kPIMetricsRepository.Update(kPIMetricToUpdate);
        await kPIMetricsRepository.SaveChangesAsync(cancellationToken);

        return kPIMetricToUpdate;
    }

    public async Task DeleteKPIMetricAsync(int id, CancellationToken cancellationToken)
    {
        var kPIMetricToDelete = await kPIMetricsRepository.GetByIdAsync(id, cancellationToken);
        if (kPIMetricToDelete == null)
        {
            throw new InvalidOperationException($"No KPI metric with Id={id} found.");
        }

        try
        {
            kPIMetricsRepository.Delete(kPIMetricToDelete);
            await kPIMetricsRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete KPI metric with Id={id}. It might be in use by other entities.", ex);
        }
    }
}
