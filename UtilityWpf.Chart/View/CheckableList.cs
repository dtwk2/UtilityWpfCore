using LiveCharts.Defaults;
using System.Collections.Generic;

namespace UtilityWpf.Chart
{
    class CheckableList
    {
        public CheckableList(bool check, List<DateTimePoint> dataPoints)
        {
            this.Check = check;
            this.DataPoints = dataPoints;
        }

        public CheckableList(bool check = false) : this(check, new List<DateTimePoint>())
        {
        }

        public List<DateTimePoint> DataPoints { get; set; } = new List<DateTimePoint>();
        public bool Check { get; set; }
    }
}
