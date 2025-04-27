using Employee.Performance.Evaluator.Application.RequestsAndResponses.KPIMetric;
using Employee.Performance.Evaluator.Core.Entities;

namespace Employee.Performance.Evaluator.Application.Abstractions;

public interface IKPIMetricsService
{
    Task<List<KPIMetric>> GetKPIMetricsAsync(CancellationToken cancellationToken);

    Task<KPIMetric?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<KPIMetric> CreateKPIMetricAsync(AddUpdateKPIMetricRequest addUpdateKPIMetricRequest, CancellationToken cancellationToken);

    Task<KPIMetric> UpdateKPIMetricAsync(int id, AddUpdateKPIMetricRequest addUpdateKPIMetricRequest, CancellationToken cancellationToken);

    Task DeleteKPIMetricAsync(int id, CancellationToken cancellationToken);
}
