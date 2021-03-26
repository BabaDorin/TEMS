using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Report
{
    public class AddReportTemplateViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public List<Option> Types { get; set; } = new List<Option>();
        public List<Option> Definitions { get; set; } = new List<Option>();
        public List<Option> Personnel { get; set; } = new List<Option>();
        public List<Option> Rooms { get; set; } = new List<Option>();
        public string SepparateBy { get; set; }
        public List<string> CommonProperties { get; set; } = new List<string>();
        public List<SpecificPropertyWrapper> SpecificProperties { get; set; } = new List<SpecificPropertyWrapper>();
        public List<string> Properties { get; set; } = new List<string>();
        public string Header { get; set; }
        public string Footer { get; set; }
        public List<Option> Signatories { get; set; } = new List<Option>();
    }

    public class SpecificPropertyWrapper
    {
        public string Type { get; set; }
        public List<string> Properties { get; set; } = new List<string>();
    }
}
