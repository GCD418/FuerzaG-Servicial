namespace CommonService.Domain.Entities;

public class VehicleReportDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Año { get; set; }
    public string Color { get; set; } = string.Empty;
    public string TipoDeVehículo { get; set; } = string.Empty;
}
