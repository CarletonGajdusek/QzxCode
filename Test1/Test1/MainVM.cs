using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Test1
{
    public class MainVM
    {

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }



        public MainVM()
        {
            //SeriesCollection = new SeriesCollection();
            //Labels = new string[] { "1", "2", "3", "4", "5", "6" };

            InitLiveCharts();
        }

        private void InitLiveCharts()
        {
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "2015",
                    Values = new ChartValues<ObservablePoint>()
                    {
                        new ObservablePoint(0, 10),
                        new ObservablePoint(2, 20),
                        new ObservablePoint(4, 10),
                    },
                    ColumnPadding = 10
                },

                new LineSeries
                {
                    Title = "2016",
                    Values = new ChartValues<ObservablePoint>()
                    {
                        new ObservablePoint(0, 10),
                        new ObservablePoint(3, 20),
                    },
                    Fill = Brushes.Transparent,
                },

                new LineSeries
                {
                    Title = "2017",
                    Values = new ChartValues<ObservablePoint>()
                    {
                        new ObservablePoint(0.1, 10),
                        new ObservablePoint(4, 20),
                    },
                    Fill = Brushes.Transparent,
                }
            };

            Labels = new[] { "Maria  Susan  Charles", "Susan", "Charles", "Frida" };
            Formatter = value => value.ToString("N");
        }


    }
}
