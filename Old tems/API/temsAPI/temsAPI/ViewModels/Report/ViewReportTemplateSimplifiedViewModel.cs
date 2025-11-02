using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.Report;

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

        public static ViewReportTemplateSimplifiedViewModel FromModel(ReportTemplate report)
        {
            return new ViewReportTemplateSimplifiedViewModel
            {
                Id = report.Id,
                CreatedBy = new Option
                {
                    Value = report.CreatedById,
                    Label = report.CreatedBy.UserName,
                },
                DateCreated = report.DateCreated,
                Description = report.Description,
                IsDefault = report.CreatedById == null,
                Name = report.Name
            };
        }
    }
}
