using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Report
{
    public class ViewReportTemplateSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Option CreatedBy { get; set; }
        public bool IsDefault { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
