using CommonService.Domain.Entities;

namespace CommonService.Domain.Ports;

public interface IVehicleReportRepository
{
    Task<IEnumerable<VehicleReportDto>> GetVehiclesByYearRangeAsync(int yearFrom, int yearTo);
}
