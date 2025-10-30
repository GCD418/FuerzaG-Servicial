using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CommonService.Domain.Ports;

namespace CommonService.Infrastructure.Reports.Builders;

public class PdfReportBuilder<T> : IReportBuilder<T> where T : class
{
    private string _title = string.Empty;
    private string _subtitle = string.Empty;
    private string? _logoPath;
    private List<T>? _data;
    private Dictionary<string, string> _filters = new();
    private Dictionary<string, string> _footer = new();
    private readonly List<(byte[] Image, string Title)> _charts = new();

    public IReportBuilder<T> SetTitle(string title)
    {
        _title = title;
        return this;
    }

    public IReportBuilder<T> SetSubtitle(string subtitle)
    {
        _subtitle = subtitle;
        return this;
    }

    public IReportBuilder<T> SetLogo(string logoPath)
    {
        _logoPath = logoPath;
        return this;
    }

    public IReportBuilder<T> SetData(List<T> data)
    {
        _data = data;
        return this;
    }

    public IReportBuilder<T> SetFilters(Dictionary<string, string> filters)
    {
        _filters = filters;
        return this;
    }

    public IReportBuilder<T> AddChart(byte[] chartImage, string chartTitle)
    {
        _charts.Add((chartImage, chartTitle));
        return this;
    }

    public IReportBuilder<T> SetFooter(Dictionary<string, string> footerData)
    {
        _footer = footerData;
        return this;
    }

    public byte[] BuildExcel()
    {
        throw new NotSupportedException("PdfReportBuilder no soporta generación de Excel. Use ExcelReportBuilder.");
    }

    public byte[] BuildPdf()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(40);
                page.PageColor(Colors.White);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public void Reset()
    {
        _title = string.Empty;
        _subtitle = string.Empty;
        _logoPath = null;
        _data = null;
        _filters.Clear();
        _footer.Clear();
        _charts.Clear();
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
                {
                    row.ConstantItem(80).Image(_logoPath);
                    row.RelativeItem().PaddingLeft(10).Column(col =>
                    {
                        col.Item().Text(_title)
                            .FontSize(20)
                            .FontColor("#E81C2E")
                            .Bold();

                        if (!string.IsNullOrEmpty(_subtitle))
                        {
                            col.Item().Text(_subtitle)
                                .FontSize(12)
                                .Italic()
                                .FontColor("#6c757d");
                        }
                    });
                }
                else
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(_title)
                            .FontSize(20)
                            .FontColor("#E81C2E")
                            .Bold();

                        if (!string.IsNullOrEmpty(_subtitle))
                        {
                            col.Item().Text(_subtitle)
                                .FontSize(12)
                                .Italic()
                                .FontColor("#6c757d");
                        }
                    });
                }
            });

            column.Item().PaddingTop(10).LineHorizontal(2).LineColor("#E81C2E");
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            if (_filters.Any())
            {
                column.Item().PaddingTop(10).Column(col =>
                {
                    col.Item().Text("Filtros Aplicados:")
                        .FontSize(11)
                        .Bold()
                        .FontColor("#202C45");

                    col.Item().PaddingTop(5).PaddingLeft(10).Column(filterCol =>
                    {
                        foreach (var filter in _filters)
                        {
                            filterCol.Item().Text($"• {filter.Key}: {filter.Value}")
                                .FontSize(9)
                                .FontColor("#495057");
                        }
                    });
                });

                column.Item().PaddingTop(10).PaddingBottom(10)
                    .LineHorizontal(1).LineColor("#dee2e6");
            }

            if (_data != null && _data.Any())
            {
                column.Item().PaddingTop(10).Element(container => ComposeTable(container));
            }

            if (_charts.Any())
            {
                column.Item().PaddingTop(20).Column(chartColumn =>
                {
                    foreach (var (chartImage, chartTitle) in _charts)
                    {
                        chartColumn.Item().Column(col =>
                        {
                            col.Item().Text(chartTitle)
                                .FontSize(12)
                                .Bold()
                                .FontColor("#202C45");

                            col.Item().PaddingTop(10).Image(chartImage).FitWidth();
                        });

                        chartColumn.Item().PaddingTop(15);
                    }
                });
            }
        });
    }

    private void ComposeTable(IContainer container)
    {
        if (_data == null || !_data.Any()) return;

        var properties = typeof(T).GetProperties();

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                foreach (var _ in properties)
                {
                    columns.RelativeColumn();
                }
            });

            table.Header(header =>
            {
                foreach (var property in properties)
                {
                    header.Cell().Element(CellStyle).Background("#202C45").Padding(8)
                        .Text(FormatPropertyName(property.Name))
                        .FontSize(10)
                        .Bold()
                        .FontColor(Colors.White);
                }
            });

            var rowIndex = 0;
            foreach (var item in _data)
            {
                var isEvenRow = rowIndex % 2 == 0;
                var bgColor = isEvenRow ? "#ffffff" : "#f8f9fa";

                foreach (var property in properties)
                {
                    var value = property.GetValue(item);
                    var displayValue = value?.ToString() ?? string.Empty;

                    table.Cell().Element(CellStyle).Background(bgColor).Padding(6)
                        .Text(displayValue)
                        .FontSize(9)
                        .FontColor("#212529");
                }

                rowIndex++;
            }
        });

        static IContainer CellStyle(IContainer container)
        {
            return container.Border(1).BorderColor("#dee2e6");
        }
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(10).LineHorizontal(1).LineColor("#dee2e6");

            column.Item().PaddingTop(10).Background("#e9ecef").Padding(10).Column(col =>
            {
                col.Item().Text("Información del Reporte")
                    .FontSize(10)
                    .Bold()
                    .FontColor("#202C45");

                if (_footer.Any())
                {
                    col.Item().PaddingTop(5).Column(footerCol =>
                    {
                        foreach (var item in _footer)
                        {
                            footerCol.Item().Text($"{item.Key}: {item.Value}")
                                .FontSize(8)
                                .FontColor("#495057");
                        }
                    });
                }

                col.Item().PaddingTop(5).Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(8)
                    .Italic()
                    .FontColor("#6c757d");
            });

            column.Item().AlignRight().Text(text =>
            {
                text.Span("Página ").FontSize(8).FontColor("#6c757d");
                text.CurrentPageNumber();
                text.Span(" de ").FontSize(8).FontColor("#6c757d");
                text.TotalPages();
            });
        });
    }

    private static string FormatPropertyName(string propertyName)
    {
        return System.Text.RegularExpressions.Regex.Replace(propertyName, "([a-z])([A-Z])", "$1 $2");
    }
}