using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public abstract class StaticFileHandler
    {
        public abstract string FolderPath { get; }

        public string CurrentDirectory
        {
            get => Directory.GetCurrentDirectory();
        }

        public string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }

        public string SanitarizeFileNameAndRemoveExtension(string fileName)
        {
            string finalFileName = fileName
                .Replace("/", "")
                .Replace("\\", "");

            return Path.GetFileNameWithoutExtension(finalFileName);
        }

        public string AddMd5Suffix(string fileName)
        {
            string finalFileName =
                fileName +
                "_" +
                EncryptionService.Md5(Guid.NewGuid().ToString());
            return finalFileName;
        }

        public string AddGuidSuffix(string fileName)
        {
            string finalFileName =
                fileName +
                "_" +
                Guid.NewGuid().ToString();
            return finalFileName;
        }

        public void DeleteFile(string filePath)
        {
            if(File.Exists(filePath))
                File.Delete(filePath);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
