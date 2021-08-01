using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Helpers.ReusableSnippets;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels.Email;

namespace temsAPI.Data.Factories.Email
{
    /// <summary>
    /// Notifies via E-mail when a bug report is created
    /// </summary>
    public class BugReportEmailBuilder : IEmailBuilder
    {
        private BugReport _bugReport;
        private AppSettings _appSettings;

        public BugReportEmailBuilder(BugReport bugReport, AppSettings appSettings)
        {
            _bugReport = bugReport;
            _appSettings = appSettings;
        }

        public async Task<SendEmailViewModel> BuildEmailModel()
        {
            var _recipients = _appSettings.BugReportMailRecipients;

            if (_recipients.IsNullOrEmpty())
                return new SendEmailViewModel();

            string currentDirectory = Directory.GetCurrentDirectory();

            SendEmailViewModel model = new()
            {
                From = "TEMS CIH Cahul",
                Subject = String.Format($"TEMS: {_bugReport.ReportType} from {_bugReport.CreatedBy.Identifier}"),
                Text = String.Format("Date created: {0}\n\n{1}",
                    _bugReport.DateCreated.ToString("dd.MMM.yyyy  hh:mm"),
                    _bugReport.Description),
                Attachments = _bugReport.GetAttachmentUris().Select(q => Path.Combine(currentDirectory, q)).ToList(),
                Addressees = _recipients.ToList()
            };

            return await Task.FromResult(model);
        }
    }
}
