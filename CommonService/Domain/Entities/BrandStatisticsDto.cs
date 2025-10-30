namespace CommonService.Domain.Entities;

public class BrandStatisticsDto
{
    public string Marca { get; set; } = string.Empty;
    public int CantidadDeVehículos { get; set; }
    public decimal Porcentaje { get; set; }
}
