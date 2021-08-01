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

        [Obsolete]
        public string AddGuidSuffix(string fileName)
        {
            string finalFileName =
                fileName +
                "_" +
                Guid.NewGuid().ToString();
            return finalFileName;
        }

        public string AddGuidSuffixKeepExtension(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            fileNameWithoutExtension = String.Format("{0}_{1}", fileNameWithoutExtension, Guid.NewGuid().ToString());
            
            return fileNameWithoutExtension + extension;
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

        public static long DirSizeBytes(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSizeBytes(di);
            }
            return size;
        }

        protected string GetStaticFilesFolderPath()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles");
        }
    }
}
