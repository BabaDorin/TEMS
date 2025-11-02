using System.Collections.Generic;
using System.IO;

namespace temsAPI.ViewModels.BugReport
{
    public class ViewBugReportViewModel
    {
        public string Id { get; set; }
        public string ReportType { get; set; }
        public string DateCreated { get; set; }
        public Option CreatedBy { get; set; }
        public string Description { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();

        public static ViewBugReportViewModel FromModel(Data.Entities.OtherEntities.BugReport model)
        {
            var vm = new ViewBugReportViewModel()
            {
                Id = model.Id,
                ReportType = model.ReportType,
                CreatedBy = new Option(model.CreatedBy?.Id, model.CreatedBy?.Identifier),
                DateCreated = model.DateCreated.ToString("dd.MM.yyyy  HH:mm"),
                Description = model.Description
            };

            var attachments = model.GetAttachmentUris();

            if (attachments == null)
                return vm;

            attachments.ForEach(q => vm.Attachments.Add(Path.GetFileName(q)));
            return vm;
        }
    }
}
