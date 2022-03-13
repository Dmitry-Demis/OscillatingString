using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscillatingString
{
    using System.Collections.Generic;

    using OxyPlot;

    public class MainViewModel
    {
        public MainViewModel()
        {
            this.Title = "Example 2";
            this.Points = new ObservableCollection<DataPoint>
            {
                new DataPoint(0, 4),
                new DataPoint(10, 13),
                new DataPoint(20, 15),
                new DataPoint(30, 16),
                new DataPoint(40, 12),
                new DataPoint(50, 12)
            };
            for (int i = 0; i < 100; i++)
            {
                Points.Add(new DataPoint(i + 10, 2 * i));
            }
        }

        public string Title { get; private set; }

        public IList<DataPoint> Points { get; private set; }
    }
}
