using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models
{
    public class ReportData
    {
        public string Name { get; set; }
        public string Header { get; set; }
        public List<ReportItemGroup> ReportItemGroups { get; set; }
        public string Footer { get; set; }
        public List<String> Signatories { get; set; }
    }
}
