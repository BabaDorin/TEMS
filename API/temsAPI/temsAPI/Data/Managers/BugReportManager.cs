using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services;
using temsAPI.ViewModels.BugReport;

namespace temsAPI.Data.Managers
{
    public class BugReportManager
    {
        EmailService _emailService;
        IdentityService _identityService;
        IUnitOfWork _unitOfWork;

        public BugReportManager(
            EmailService emailService,
            IdentityService identityService,
            IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Processes the bug report, which includes saving the report & notifying 
        /// </summary>
        /// <returns>Null if everything is ok, otherwise - returns an error message</returns>
        public async Task<string> ProcessBugReport(BugReportViewModel viewModel)
        {
            var bugReport = await CreateBugReport(viewModel);
            await NotifyBugReport(bugReport);
            return null;
        }

        private async Task<BugReport> CreateBugReport(BugReportViewModel viewModel)
        {
            BugReport bugReport = new()
            {
                Id = Guid.NewGuid().ToString(),
                ReportType = viewModel.ReportType,
                Description = viewModel.Description,
                DateCreated = DateTime.Now,
                CreatedByID = _identityService.GetUserId(),
                CreatedBy = await _identityService.GetCurrentUserAsync(),
            };

            var attachmentRelativePaths = await SaveAttachments(viewModel.Attachments);
            bugReport.SetAttachments(attachmentRelativePaths);

            await _unitOfWork.BugReports.Create(bugReport);
            await _unitOfWork.Save();

            return bugReport;
        }

        /// <summary>
        /// Saves report's attachments to disk and returns a list containing attachment relative paths
        /// </summary>
        /// <param name="attachments"></param>
        /// <returns></returns>
        private async Task<List<string>> SaveAttachments(List<IFormFile> attachments)
        {
            if (attachments == null || attachments.Count == 0)
                return new List<string>();

            BugReportAttachmentFileHandler attachmentFileHandler = new();
            List<string> attachmentRelativePaths = new();

            foreach(var attachment in attachments)
            {
                string attachmentPathAfterSaving = await attachmentFileHandler.SaveAttachment(attachment);
                attachmentRelativePaths.Add(attachmentPathAfterSaving);
            }

            return attachmentRelativePaths;
        }

        /// <summary>
        /// Sends an email with attachments to e-mail addresses specified in appsettings.json
        /// </summary>
        /// <param name="report"></param>
        /// <returns>Null if everything is OK, otherwise - return an error message</returns>
        private async Task<string> NotifyBugReport(BugReport report)
        {
            //await Task.Yield();
            return "not implemented";
        }
    }
}
