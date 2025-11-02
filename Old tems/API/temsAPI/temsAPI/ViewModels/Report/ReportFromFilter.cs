using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Helpers.Filters;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.ViewModels.Report
{
    public class ReportFromFilter
    {
        public string Name { get; set; }
        public string Header { get; set; }
        public List<string> CommonProperties { get; set; }
        public string Footer { get; set; }
        public List<string> Signatories { get; set; }
        public EquipmentFilter Filter { get; set; }

        public string Validate()
        {
            if (CommonProperties.IsNullOrEmpty())
                return "At least one property is required to generate the report.";

            if (Filter == null)
                return "A valid filter has not been defined";

            return null;
        }
    }
}
