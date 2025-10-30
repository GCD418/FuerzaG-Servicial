using CommonService.Domain.Ports;
using ScottPlot;

namespace CommonService.Infrastructure.Reports;

public class ChartGenerator : IChartGenerator
{
    private const int DefaultWidth = 800;
    private const int DefaultHeight = 500;

    public byte[] GenerateBarChart(List<string> labels, List<double> values, string title, string xLabel, string yLabel)
    {
        var plot = new Plot();
        
        var bar = plot.Add.Bars(values.ToArray());
        
        foreach (var b in bar.Bars)
        {
            b.FillColor = ScottPlot.Color.FromHex("#E81C2E"); // Color principal de FuerzaG
        }
        
        plot.Title(title);
        plot.XLabel(xLabel);
        plot.YLabel(yLabel);
        
        double[] positions = Enumerable.Range(0, labels.Count).Select(i => (double)i).ToArray();
        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            positions,
            labels.ToArray()
        );
        
        plot.Axes.Bottom.MajorTickStyle.Length = 0;
        plot.HideGrid();
        
        return plot.GetImage(DefaultWidth, DefaultHeight).GetImageBytes();
    }

    public byte[] GeneratePieChart(List<string> labels, List<double> values, string title)
    {
        var plot = new Plot();
        
        var pie = plot.Add.Pie(values);
        pie.ExplodeFraction = 0.05;
        
        for (int i = 0; i < labels.Count; i++)
        {
            pie.Slices[i].Label = labels[i];
        }
        
        var colors = new[]
        {
            ScottPlot.Color.FromHex("#E81C2E"), // Rojo FuerzaG
            ScottPlot.Color.FromHex("#202C45"), // Azul oscuro
            ScottPlot.Color.FromHex("#F8B500"), // Amarillo
            ScottPlot.Color.FromHex("#2E8B57"), // Verde
            ScottPlot.Color.FromHex("#4169E1"), // Azul real
            ScottPlot.Color.FromHex("#8B008B"), // Magenta oscuro
            ScottPlot.Color.FromHex("#FF6347"), // Tomate
            ScottPlot.Color.FromHex("#20B2AA"), // Verde azulado
        };
        
        for (int i = 0; i < pie.Slices.Count && i < colors.Length; i++)
        {
            pie.Slices[i].FillColor = colors[i];
        }
        
        plot.Title(title);
        plot.HideGrid();
        plot.Layout.Frameless();
        
        plot.ShowLegend();
        
        return plot.GetImage(DefaultWidth, DefaultHeight).GetImageBytes();
    }

    public byte[] GenerateLineChart(List<string> labels, List<double> values, string title, string xLabel, string yLabel)
    {
        var plot = new Plot();
        
        double[] positions = Enumerable.Range(0, values.Count).Select(i => (double)i).ToArray();
        
        var line = plot.Add.Scatter(positions, values.ToArray());
        line.Color = ScottPlot.Color.FromHex("#E81C2E");
        line.LineWidth = 2;
        line.MarkerSize = 8;
        
        plot.Title(title);
        plot.XLabel(xLabel);
        plot.YLabel(yLabel);
        
        plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(
            positions,
            labels.ToArray()
        );
        
        plot.ShowGrid();
        
        return plot.GetImage(DefaultWidth, DefaultHeight).GetImageBytes();
    }
}