using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Controllers.IdentityControllers;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Factories.Email;
using temsAPI.Data.Factories.Notification;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.BugReport;
using temsAPI.ViewModels.Report;

namespace temsAPI.Data.Managers
{
    public class BugReportManager
    {
        EmailService _emailService;
        IdentityService _identityService;
        IUnitOfWork _unitOfWork;
        SystemConfigurationService _configurationService;
        UserManager<TEMSUser> _userManager;
        NotificationManager _notificationManager;

        public BugReportManager(
            EmailService emailService,
            IdentityService identityService,
            IUnitOfWork unitOfWork,
            SystemConfigurationService configurationService,
            UserManager<TEMSUser> userManager,
            NotificationManager notificationManager)
        {
            _emailService = emailService;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _configurationService = configurationService;
            _userManager = userManager;
            _notificationManager = notificationManager;
        }

        /// <summary>
        /// Processes the bug report, which includes saving the report & notifying 
        /// </summary>
        /// <returns>Null if everything is ok, otherwise - an error message</returns>
        public async Task<string> ProcessBugReport(BugReportViewModel viewModel)
        {
            var bugReport = await CreateBugReport(viewModel);
            var notificationResult = await NotifyBugReport(bugReport);
            if (notificationResult != null && !int.TryParse(notificationResult, out _))
                return notificationResult;
            
            return null;
        }

        /// <summary>
        /// Creates the bug report model, saves attachments to disk, saves the model into the db
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns>The resulted BugReport model</returns>
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
        /// and creates a notification addressed to system administrators
        /// </summary>
        /// <param name="report"></param>
        /// <returns>Null if everything is OK, otherwise - return an error message</returns>
        private async Task<string> NotifyBugReport(BugReport report)
        {
            string result = await NotifyBugReportEmail(report);
            await _notificationManager.NotifyBugReport(report);
            return result;
        }

        /// <summary>
        /// Notifies about bug report via e-mail
        /// </summary>
        /// <param name="report"></param>
        /// <returns>Null if everything is ok, otherwise - an error message</returns>
        private async Task<string> NotifyBugReportEmail(BugReport report)
        {
            var emailBuilder = new BugReportEmailBuilder(report, _configurationService.AppSettings);
            var emailModel = await emailBuilder.BuildEmailModel();
            return await _emailService.SendEmailToAddresses(emailModel);
        }

        public async Task<ViewBugReportViewModel> GetFullBugReport(string reportId)
        {
            return (await _unitOfWork.BugReports
                .Find(
                    where: q => q.Id == reportId,
                    include: q => q.Include(q => q.CreatedBy),
                    select: q => ViewBugReportViewModel.FromModel(q)))
                .FirstOrDefault();
        }

        public async Task<List<Option>> GetBugReportsSimplified(int skip, int take)
        {
            return (await _unitOfWork.BugReports
                .FindAll(
                    skip: skip,
                    take: take,
                    orderBy: q => q.OrderByDescending(q => q.DateCreated),
                    include: q => q.Include(q => q.CreatedBy),
                    select: q => new Option(
                        q.Id, 
                        q.Identifier, 
                        q.DateCreated.ToString("dd.MM.yyyy  HH:mm"))
                ))
                .ToList();
        }

        public async Task<int> GetTotalBugReportsAmount()
        {
            return await _unitOfWork.BugReports.Count();
        }

        /// <summary>
        /// Removes the specified bug report
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns>Null if everything is ok, otherwise - an error message</returns>
        public async Task<string> RemoveReport(string reportId)
        {
            var toRemove = await GetReportById(reportId);
            if (toRemove == null)
                return "Invalid id provided";

            _unitOfWork.BugReports.Delete(toRemove);
            await _unitOfWork.Save();
            return null;
        }

        private async Task<BugReport> GetReportById(string reportId)
        {
            return (await _unitOfWork.BugReports
                .Find<BugReport>(q => q.Id == reportId))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets attachment's memory stream and content type
        /// </summary>
        /// <param name="reportId"></param>
        /// <param name="attachmentIndex"></param>
        /// <returns>Attachment's Memory stream and content type</returns>
        public async Task<(MemoryStream, string)> GetAttachmentData(string reportId, int attachmentIndex)
        {
            var report = await GetReportById(reportId);

            BugReportAttachmentFileHandler fileHandler = new();
            var attachmentPath = fileHandler.GetFullAttachmentPath(report.GetAttachmentUris()[attachmentIndex]);
            
            if (!fileHandler.FileExists(attachmentPath))
                throw new Exception("File does not exist");

            byte[] fileBytes = File.ReadAllBytes(attachmentPath);
            
            MemoryStream ms = new MemoryStream(fileBytes);
            var contentType = fileHandler.GetContentType(attachmentPath);
            return (ms, contentType);
        }
    }
}
