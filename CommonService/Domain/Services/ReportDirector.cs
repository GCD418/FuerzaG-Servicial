using CommonService.Domain.Ports;

namespace CommonService.Domain.Services;

public class ReportDirector
{
    public byte[] ConstructReport<T>(
        IReportBuilder<T> builder,
        List<T> data,
        string title,
        string subtitle,
        string logoPath,
        Dictionary<string, string> filters,
        Dictionary<string, string> footer,
        ReportFormat format,
        byte[]? chartImage = null,
        string? chartTitle = null) where T : class
    {
        builder.Reset();
        
        builder.SetTitle(title)
            .SetSubtitle(subtitle)
            .SetLogo(logoPath)
            .SetData(data)
            .SetFilters(filters);

        if (chartImage != null && !string.IsNullOrEmpty(chartTitle))
        {
            builder.AddChart(chartImage, chartTitle);
        }

        builder.SetFooter(footer);

        return format switch
        {
            ReportFormat.Pdf => builder.BuildPdf(),
            ReportFormat.Excel => builder.BuildExcel(),
            _ => throw new ArgumentException("Formato de reporte no soportado", nameof(format))
        };
    }

    public byte[] ConstructSimpleReport<T>(
        IReportBuilder<T> builder,
        List<T> data,
        string title,
        Dictionary<string, string> filters,
        ReportFormat format) where T : class
    {
        var footer = new Dictionary<string, string>
        {
            ["Total de registros"] = data.Count.ToString()
        };

        return ConstructReport(
            builder,
            data,
            title,
            string.Empty,
            string.Empty,
            filters,
            footer,
            format);
    }
}
