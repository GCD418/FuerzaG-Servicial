using CommonService.Domain.Services;
using FuerzaGServicial.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuerzaGServicial.Pages.Reports;

[Authorize(Roles = "Manager,CEO")]
public class BrandStatisticsModel : PageModel
{
    private readonly BrandReportService _reportService;

    public BrandStatisticsModel(BrandReportService reportService)
    {
        _reportService = reportService;
    }

    [BindProperty]
    public int MinCount { get; set; } = 1;

    [BindProperty]
    public int MaxCount { get; set; } = 100;

    [BindProperty]
    public string Format { get; set; } = "pdf";

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostGenerateAsync()
    {
        if (MinCount > MaxCount)
        {
            ErrorMessage = "La cantidad mínima no puede ser mayor a la cantidad máxima.";
            return Page();
        }

        if (MinCount < 0 || MaxCount < 0)
        {
            ErrorMessage = "Las cantidades deben ser valores positivos.";
            return Page();
        }

        if (MaxCount > 10000)
        {
            ErrorMessage = "La cantidad máxima no puede superar 10,000 vehículos.";
            return Page();
        }

        try
        {
            var format = Format.ToLower() == "pdf" ? ReportFormat.Pdf : ReportFormat.Excel;
            var reportBytes = await _reportService.GenerateReportAsync(MinCount, MaxCount, format);
            var fileName = _reportService.GetFileName(format);
            var contentType = _reportService.GetContentType(format);

            return File(reportBytes, contentType, fileName);
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception)
        {
            ErrorMessage = "Ocurrió un error al generar el reporte. Por favor, intente nuevamente.";
            return Page();
        }
    }
}
