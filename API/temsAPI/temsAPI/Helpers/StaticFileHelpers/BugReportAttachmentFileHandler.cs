using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public class BugReportAttachmentFileHandler : StaticFileHandler
    {
        public override string FolderPath { get; }

        public BugReportAttachmentFileHandler()
        {
            FolderPath = Path.Combine(GetStaticFilesFolderPath(), "BugReportAttachments");
        }

        /// <summary>
        /// Saves an attachment and returns the attachment path from current directory
        /// </summary>
        /// <returns></returns>
        public async Task<string> SaveAttachment(IFormFile file)
        {
            if (file.Length == 0)
                throw new ArgumentException("Invalid file: " + file.FileName);
            
            string fileName = AddGuidSuffixKeepExtension(file.FileName);
            string filePath = Path.Combine(FolderPath, fileName);

            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return GetRelativeAttachmentPath(fileName);
        }

        private string GetRelativeAttachmentPath(string attachmentName)
        {
            return Path.Combine("StaticFiles", "BugReportAttachments", attachmentName);
        }

        public string GetFullAttachmentPath(string attachment)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), attachment);
        }

        public void RemoveAttachments(BugReport report)
        {
            if (String.IsNullOrEmpty(report.Attachments))
                return;

            var attachments = report.GetAttachmentUris();

            foreach(var att in attachments)
            {
                RemoveAttachment(att);
            }
        }

        public void RemoveAttachment(string attachmentRelativePath)
        {
            string fullAttachmentPath = GetFullAttachmentPath(attachmentRelativePath);
            if (FileExists(fullAttachmentPath))
                File.Delete(fullAttachmentPath);
        }
    }
}
