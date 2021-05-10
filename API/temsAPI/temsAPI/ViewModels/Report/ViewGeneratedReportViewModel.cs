using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.Report;

namespace temsAPI.ViewModels.Report
{
    public class ViewGeneratedReportViewModel
    {
        public string Id { get; set; }
        public string Template { get; set; }
        public Option GeneratedBy { get; set; }
        public DateTime DateGenerated { get; set; }

        public static ViewGeneratedReportViewModel FromModel(Data.Entities.Report.Report report)
        {
            return new ViewGeneratedReportViewModel
            {
                Id = report.Id,
                DateGenerated = report.DateGenerated,
                GeneratedBy = new Option
                {
                    Value = report.GeneratedBy.Id,
                    Label = report.GeneratedBy.FullName ?? report.GeneratedBy.UserName
                },
                Template = report.Template
            };
        }
    }
}
