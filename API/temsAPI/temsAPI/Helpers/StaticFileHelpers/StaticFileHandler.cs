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
                EncryptionService.Md5(DateTime.Now.ToString());
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
            File.Delete(filePath);
        }
    }
}
