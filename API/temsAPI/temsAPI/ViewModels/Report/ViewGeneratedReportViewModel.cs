using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Report
{
    public class ViewGeneratedReportViewModel
    {
        public string Id { get; set; }
        public string Template { get; set; }
        public Option GeneratedBy { get; set; }
        public DateTime DateGenerated { get; set; }
    }
}
