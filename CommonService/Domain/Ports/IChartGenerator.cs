namespace CommonService.Domain.Ports;

public interface IChartGenerator
{
    byte[] GenerateBarChart(List<string> labels, List<double> values, string title, string xLabel, string yLabel);
    
    byte[] GeneratePieChart(List<string> labels, List<double> values, string title);
    
    byte[] GenerateLineChart(List<string> labels, List<double> values, string title, string xLabel, string yLabel);
}