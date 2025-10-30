namespace CommonService.Domain.Ports;

public interface IReportBuilder<T> where T : class
{
    IReportBuilder<T> SetTitle(string title);

    IReportBuilder<T> SetSubtitle(string subtitle);

    IReportBuilder<T> SetLogo(string logoPath);

    IReportBuilder<T> SetData(List<T> data);

    IReportBuilder<T> SetFilters(Dictionary<string, string> filters);
    
    IReportBuilder<T> AddChart(byte[] chartImage, string chartTitle);
    
    IReportBuilder<T> SetFooter(Dictionary<string, string> footerData);
    
    byte[] BuildPdf();
    
    byte[] BuildExcel();
    
    void Reset();
}