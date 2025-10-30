using ClosedXML.Excel;
using CommonService.Domain.Ports;

namespace CommonService.Infrastructure.Reports.Builders;

public class ExcelReportBuilder<T> : IReportBuilder<T> where T : class
{
    private XLWorkbook? _workbook;
    private IXLWorksheet? _worksheet;
    private string _title = string.Empty;
    private string _subtitle = string.Empty;
    private string? _logoPath;
    private List<T>? _data;
    private Dictionary<string, string> _filters = new();
    private Dictionary<string, string> _footer = new();
    private readonly List<(byte[] Image, string Title)> _charts = new();
    private int _currentRow = 1;

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

    public byte[] BuildPdf()
    {
        throw new NotSupportedException("ExcelReportBuilder no soporta generación de PDF. Use PdfReportBuilder.");
    }

    public byte[] BuildExcel()
    {
        _workbook = new XLWorkbook();
        _worksheet = _workbook.Worksheets.Add("Reporte");
        _currentRow = 1;

        BuildHeader();
        BuildFilters();
        BuildDataTable();
        BuildCharts();
        BuildFooter();
        ApplyFormatting();

        using var stream = new MemoryStream();
        _workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public void Reset()
    {
        _workbook?.Dispose();
        _workbook = null;
        _worksheet = null;
        _title = string.Empty;
        _subtitle = string.Empty;
        _logoPath = null;
        _data = null;
        _filters.Clear();
        _footer.Clear();
        _charts.Clear();
        _currentRow = 1;
    }

    private void BuildHeader()
    {
        if (_worksheet == null) return;

        if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
        {
            var logo = _worksheet.AddPicture(_logoPath)
                .MoveTo(_worksheet.Cell(_currentRow, 1))
                .Scale(0.15);
            _currentRow += 4;
        }

        var titleCell = _worksheet.Cell(_currentRow, 1);
        titleCell.Value = _title;
        titleCell.Style.Font.FontSize = 18;
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontColor = XLColor.FromHtml("#E81C2E");
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        
        var properties = typeof(T).GetProperties();
        _worksheet.Range(_currentRow, 1, _currentRow, properties.Length).Merge();
        _currentRow++;

        if (!string.IsNullOrEmpty(_subtitle))
        {
            var subtitleCell = _worksheet.Cell(_currentRow, 1);
            subtitleCell.Value = _subtitle;
            subtitleCell.Style.Font.FontSize = 12;
            subtitleCell.Style.Font.Italic = true;
            subtitleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            _worksheet.Range(_currentRow, 1, _currentRow, properties.Length).Merge();
            _currentRow++;
        }

        _currentRow++;
    }

    private void BuildFilters()
    {
        if (_worksheet == null || !_filters.Any()) return;

        var filterCell = _worksheet.Cell(_currentRow, 1);
        filterCell.Value = "Filtros Aplicados:";
        filterCell.Style.Font.Bold = true;
        filterCell.Style.Font.FontSize = 11;
        _currentRow++;

        foreach (var filter in _filters)
        {
            var cell = _worksheet.Cell(_currentRow, 1);
            cell.Value = $"{filter.Key}: {filter.Value}";
            cell.Style.Font.FontSize = 10;
            _currentRow++;
        }

        _currentRow++;
    }

    private void BuildDataTable()
    {
        if (_worksheet == null || _data == null || !_data.Any()) return;

        var properties = typeof(T).GetProperties();
        var startRow = _currentRow;

        for (int i = 0; i < properties.Length; i++)
        {
            var headerCell = _worksheet.Cell(_currentRow, i + 1);
            headerCell.Value = FormatPropertyName(properties[i].Name);
            headerCell.Style.Font.Bold = true;
            headerCell.Style.Font.FontColor = XLColor.White;
            headerCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#202C45");
            headerCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
        _currentRow++;

        foreach (var item in _data)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                var cell = _worksheet.Cell(_currentRow, i + 1);
                var value = properties[i].GetValue(item);
                
                if (value != null)
                {
                    if (value is decimal decimalValue)
                        cell.Value = decimalValue;
                    else if (value is int intValue)
                        cell.Value = intValue;
                    else if (value is double doubleValue)
                        cell.Value = doubleValue;
                    else
                        cell.Value = value.ToString();
                }

                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                
                if (_currentRow % 2 == 0)
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8F9FA");
            }
            _currentRow++;
        }

        var table = _worksheet.Range(startRow, 1, _currentRow - 1, properties.Length);
        table.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        
        _currentRow++;
    }

    private void BuildCharts()
    {
        if (_worksheet == null || !_charts.Any()) return;

        foreach (var (chartImage, chartTitle) in _charts)
        {
            var titleCell = _worksheet.Cell(_currentRow, 1);
            titleCell.Value = chartTitle;
            titleCell.Style.Font.Bold = true;
            titleCell.Style.Font.FontSize = 12;
            _currentRow++;

            try
            {
                using var ms = new MemoryStream(chartImage);
                var picture = _worksheet.AddPicture(ms)
                    .MoveTo(_worksheet.Cell(_currentRow, 1))
                    .Scale(0.5);
                
                _currentRow += 20;
            }
            catch
            {
                var errorCell = _worksheet.Cell(_currentRow, 1);
                errorCell.Value = "[Gráfico no disponible]";
                errorCell.Style.Font.Italic = true;
                _currentRow++;
            }
        }

        _currentRow++;
    }

    private void BuildFooter()
    {
        if (_worksheet == null) return;

        _currentRow++;
        
        var footerStartRow = _currentRow;
        var footerCell = _worksheet.Cell(_currentRow, 1);
        footerCell.Value = "Información del Reporte";
        footerCell.Style.Font.Bold = true;
        footerCell.Style.Font.FontSize = 11;
        footerCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E9ECEF");
        _currentRow++;

        if (_footer.Any())
        {
            foreach (var item in _footer)
            {
                var cell = _worksheet.Cell(_currentRow, 1);
                cell.Value = $"{item.Key}: {item.Value}";
                cell.Style.Font.FontSize = 9;
                _currentRow++;
            }
        }

        var dateCell = _worksheet.Cell(_currentRow, 1);
        dateCell.Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
        dateCell.Style.Font.FontSize = 9;
        dateCell.Style.Font.Italic = true;
    }

    private void ApplyFormatting()
    {
        if (_worksheet == null) return;

        _worksheet.Columns().AdjustToContents();
        
        foreach (var column in _worksheet.ColumnsUsed())
        {
            if (column.Width > 50)
                column.Width = 50;
            if (column.Width < 15)
                column.Width = 15;
        }
    }

    private static string FormatPropertyName(string propertyName)
    {
        return System.Text.RegularExpressions.Regex.Replace(propertyName, "([a-z])([A-Z])", "$1 $2");
    }
}