using CommonService.Domain.Services;
using FuerzaGServicial.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FuerzaGServicial.Pages.Reports;

[Authorize(Roles = "Manager,CEO")]
public class VehicleReportModel : PageModel
{
    private readonly VehicleReportService _reportService;

    public VehicleReportModel(VehicleReportService reportService)
    {
        _reportService = reportService;
    }

    [BindProperty]
    public int YearFrom { get; set; } = DateTime.Now.Year - 10;

    [BindProperty]
    public int YearTo { get; set; } = DateTime.Now.Year;

    [BindProperty]
    public string Format { get; set; } = "pdf";

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostGenerateAsync()
    {
        if (YearFrom > YearTo)
        {
            ErrorMessage = "El año inicial no puede ser mayor al año final.";
            return Page();
        }

        if (YearFrom < 1900 || YearTo > DateTime.Now.Year + 1)
        {
            ErrorMessage = "Rango de años inválido.";
            return Page();
        }

        try
        {
            var format = Format.ToLower() == "pdf" ? ReportFormat.Pdf : ReportFormat.Excel;
            var reportBytes = await _reportService.GenerateReportAsync(YearFrom, YearTo, format);
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
