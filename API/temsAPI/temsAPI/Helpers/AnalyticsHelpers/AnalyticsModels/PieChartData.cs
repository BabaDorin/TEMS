using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers.AnalyticsHelpers.AnalyticsModels
{
    public class PieChartData
    {
        public string ChartName { get; set; }
        public List<Tuple<string, int>> Rates { get; set; }
    }
}
