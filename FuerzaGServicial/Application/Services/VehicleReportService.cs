using CommonService.Domain.Entities;
using CommonService.Domain.Ports;
using CommonService.Domain.Services;
using CommonService.Infrastructure.Reports.Builders;

namespace FuerzaGServicial.Application.Services;

public class VehicleReportService
{
    private readonly IVehicleReportRepository _repository;
    private readonly ReportDirector _director;
    private readonly IWebHostEnvironment _environment;

    public VehicleReportService(
        IVehicleReportRepository repository,
        ReportDirector director,
        IWebHostEnvironment environment)
    {
        _repository = repository;
        _director = director;
        _environment = environment;
    }

    public async Task<byte[]> GenerateReportAsync(int yearFrom, int yearTo, ReportFormat format)
    {
        var data = await _repository.GetVehiclesByYearRangeAsync(yearFrom, yearTo);
        var dataList = data.ToList();

        if (!dataList.Any())
        {
            throw new InvalidOperationException("No se encontraron vehículos en el rango de años especificado.");
        }

        var filters = new Dictionary<string, string>
        {
            ["Año Desde"] = yearFrom.ToString(),
            ["Año Hasta"] = yearTo.ToString()
        };

        var footer = new Dictionary<string, string>
        {
            ["Total de Vehículos"] = dataList.Count.ToString(),
            ["Rango de Años"] = $"{yearFrom} - {yearTo}",
            ["Marcas Únicas"] = dataList.Select(v => v.BrandName).Distinct().Count().ToString()
        };

        var logoPath = Path.Combine(_environment.WebRootPath, "img", "logo-fuerzaG.png");

        IReportBuilder<VehicleReportDto> builder = format == ReportFormat.Pdf
            ? new PdfReportBuilder<VehicleReportDto>()
            : new ExcelReportBuilder<VehicleReportDto>();

        return _director.ConstructReport(
            builder,
            dataList,
            "Reporte de Vehículos por Rango de Años",
            $"Listado de vehículos registrados entre {yearFrom} y {yearTo}",
            logoPath,
            filters,
            footer,
            format);
    }

    public string GetFileName(ReportFormat format)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var extension = format == ReportFormat.Pdf ? "pdf" : "xlsx";
        return $"Reporte_Vehiculos_{timestamp}.{extension}";
    }

    public string GetContentType(ReportFormat format)
    {
        return format == ReportFormat.Pdf
            ? "application/pdf"
            : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    }
}
