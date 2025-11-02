using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Helpers.SystemModels;

namespace temsAPI.ViewModels.BugReport
{
    public class BugReportViewModel
    {
        public string ReportType { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
