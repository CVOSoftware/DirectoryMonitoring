using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace DirectoryMonitoring.Studio.Chart
{
    internal class ChartData
    {
        private const int DEFAULT_COUNT = 0;

        private const double FILL_OPACITY = 0.1;

        public ChartData(Brush stroke)
        {
            var fill = stroke.Clone();

            fill.Opacity = FILL_OPACITY;
            Series = new LineSeries();
            Series.Values = new ChartValues<int>();
            Series.Stroke = stroke;
            Series.Fill = fill;
        }

        public int Count { get; set; }

        public LineSeries Series { get; set; }

        public void UpdateChart()
        {
            Series.Values.Add(Count);
            Count = DEFAULT_COUNT;
        }
    }
}
