using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
