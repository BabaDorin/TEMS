using System.Collections.Generic;

namespace ReportGenerator.Models
{
    public class ReportData
    {
        public string GeneratedBy { get; set; }
        public string Name { get; set; }
        public string Header { get; set; }
        public List<ReportItemGroup> ReportItemGroups { get; set; }
        public string Footer { get; set; }
        public List<string> Signatories { get; set; }
    }
}
