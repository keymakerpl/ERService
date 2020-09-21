using LiveCharts;
using System;
using LiveCharts.Wpf.Charts.Base;

namespace ERService.Infrastructure.Extensions
{
    public static class ChartExtenssions
    {
        public static Chart GetChartForPDF<T>(this Chart chart) where T : Chart
        {
            var newChart = Activator.CreateInstance(chart.GetType()) as T;
            newChart.DisableAnimations = true;
            newChart.Width = 640;
            newChart.Height = 480;
            newChart.LegendLocation = LegendLocation.Bottom;
            newChart.Series = chart.Series;

            return newChart;
        }
    }
}