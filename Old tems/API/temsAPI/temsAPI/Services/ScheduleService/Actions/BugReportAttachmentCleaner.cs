using FluentEmail.Core;
using Microsoft.OpenApi.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Helpers.ReusableSnippets;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services.Actions;

namespace temsAPI.Services.ScheduleService.Actions
{
    // BEFREE: TEST THIS THING!
    public class BugReportAttachmentCleaner : IScheduledAction
    {
        private IUnitOfWork _unitOfWork;
        private BugReportAttachmentFileHandler _fileHandler;

        public BugReportAttachmentCleaner(
            IUnitOfWork unitOfWork,
            BugReportAttachmentFileHandler fileHandler)
        {
            _unitOfWork = unitOfWork;
            _fileHandler = fileHandler;
        }

        public async Task Start()
        {
            await SanitarizeAttachmentFolder();
        }

        /// <summary>
        /// Removes innacessible attachments from bug report attachments folder (for which the report does not exist anymore)
        /// </summary>
        /// <returns></returns>
        private async Task SanitarizeAttachmentFolder()
        {
            IEnumerable<string> totalAttachments = Enumerable.Empty<string>();

            (await _unitOfWork.BugReports
                .FindAll<BugReport>())
                .ForEach(q => totalAttachments = totalAttachments
                    .Concat(q.GetAttachmentUris().Select(att => _fileHandler.GetFullAttachmentPath(att))
                    .ToList()));

            if (totalAttachments.IsNullOrEmpty())
                return;
            
            var files = Directory.GetFiles(_fileHandler.FolderPath);
            var innacessibleFiles = files.Except(totalAttachments);

            if (innacessibleFiles.IsNullOrEmpty())
                return;

            foreach (var file in innacessibleFiles)
            {
                File.Delete(file);
            }
        }
    }
}
