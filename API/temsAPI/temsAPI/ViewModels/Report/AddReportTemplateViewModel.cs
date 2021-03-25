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
        public List<Option> Types { get; set; }
        public List<Option> Definitions { get; set; }
        public List<Option> Personnel { get; set; }
        public List<Option> Rooms { get; set; }
        public string SepparateBy { get; set; }
        public List<string> CommonProperties { get; set; }
        public List<SpecificPropertyWrapper> SpecificProperties { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        public List<Option> Signatories { get; set; }
    }

    public class SpecificPropertyWrapper
    {
        public string Type { get; set; }
        public List<string> Properties { get; set; }
    }
}
