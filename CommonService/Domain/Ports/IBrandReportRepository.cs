using CommonService.Domain.Entities;

namespace CommonService.Domain.Ports;

public interface IBrandReportRepository
{
    Task<IEnumerable<BrandStatisticsDto>> GetBrandStatisticsByVehicleCountAsync(int minCount, int maxCount);
}
