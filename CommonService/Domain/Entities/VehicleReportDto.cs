namespace CommonService.Domain.Entities;

public class VehicleReportDto
{
    public string OwnerFullName { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
}
