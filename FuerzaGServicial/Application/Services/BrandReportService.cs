using CommonService.Domain.Entities;
using CommonService.Domain.Ports;
using CommonService.Domain.Services;
using CommonService.Infrastructure.Reports.Builders;

namespace FuerzaGServicial.Application.Services;

public class BrandReportService
{
    private readonly IBrandReportRepository _repository;
    private readonly IChartGenerator _chartGenerator;
    private readonly ReportDirector _director;
    private readonly IWebHostEnvironment _environment;

    public BrandReportService(
        IBrandReportRepository repository,
        IChartGenerator chartGenerator,
        ReportDirector director,
        IWebHostEnvironment environment)
    {
        _repository = repository;
        _chartGenerator = chartGenerator;
        _director = director;
        _environment = environment;
    }

    public async Task<byte[]> GenerateReportAsync(int minCount, int maxCount, ReportFormat format, string userFullName)
    {
        var data = await _repository.GetBrandStatisticsByVehicleCountAsync(minCount, maxCount);
        var dataList = data.ToList();

        if (!dataList.Any())
        {
            throw new InvalidOperationException("No se encontraron marcas en el rango de vehículos especificado.");
        }

        var labels = dataList.Select(d => d.Marca).ToList();
        var values = dataList.Select(d => (double)d.CantidadDeVehículos).ToList();

        var chartImage = _chartGenerator.GeneratePieChart(
            labels,
            values,
            "Distribución de Vehículos por Marca");

        var filters = new Dictionary<string, string>
        {
            ["Cantidad Mínima"] = minCount.ToString(),
            ["Cantidad Máxima"] = maxCount.ToString()
        };

        var totalVehicles = dataList.Sum(d => d.CantidadDeVehículos);
        var footer = new Dictionary<string, string>
        {
            ["Total de Marcas"] = dataList.Count.ToString(),
            ["Total de Vehículos"] = totalVehicles.ToString(),
            ["Marca con Más Vehículos"] = dataList.First().Marca,
            ["Cantidad Máxima"] = dataList.First().CantidadDeVehículos.ToString(),
            ["Generado por"] = userFullName
        };

        var logoPath = Path.Combine(_environment.WebRootPath, "img", "logo-fuerzaG.png");

        IReportBuilder<BrandStatisticsDto> builder = format == ReportFormat.Pdf
            ? new PdfReportBuilder<BrandStatisticsDto>()
            : new ExcelReportBuilder<BrandStatisticsDto>();

        return _director.ConstructReport(
            builder,
            dataList,
            "Reporte Estadístico de Marcas",
            $"Marcas con {minCount} a {maxCount} vehículos registrados",
            logoPath,
            filters,
            footer,
            format,
            chartImage,
            "Distribución de Vehículos por Marca");
    }

    public string GetFileName(ReportFormat format)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var extension = format == ReportFormat.Pdf ? "pdf" : "xlsx";
        return $"Reporte_Estadisticas_Marcas_{timestamp}.{extension}";
    }

    public string GetContentType(ReportFormat format)
    {
        return format == ReportFormat.Pdf
            ? "application/pdf"
            : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
