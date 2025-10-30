namespace CommonService.Domain.Entities;

public class BrandStatisticsDto
{
    public string BrandName { get; set; } = string.Empty;
    public int VehicleCount { get; set; }
    public decimal Percentage { get; set; }
}
