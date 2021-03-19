using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace temsAPI.Helpers
{
    public class FileUploadService
    {
        public static string CurrentDirectory
        {
            get => Directory.GetCurrentDirectory();
        }

        public static string LibraryFolderName
        {
            get => Path.Combine("StaticFiles", "LibraryUploads");
        }

        public static string PathToSave
        {
            get
            {
                return Path.Combine(
                    CurrentDirectory,
                    Path.Combine("StaticFiles", "LibraryUploads")
                    );
            }
        }
        public static string SanitarizeFileNameAndRemoveExtension(string fileName)
        {
            string finalFileName = fileName
                .Replace("/", "")
                .Replace("\\", "");

            return Path.GetFileNameWithoutExtension(finalFileName);
        }

        public static string AddMd5Suffix(string fileName)
        {
            string finalFileName = 
                fileName + 
                "_" + 
                EncryptionService.Md5(DateTime.Now.ToString());
            return finalFileName;
        }

        public static string CompressAndSave(IFormFile file, string actualName)
        {
            try
            {
                string dbPath = null;
                if (file.Length > 0)
                {
                    dbPath = String.Concat(
                        Path.Combine(FileUploadService.LibraryFolderName, actualName),
                        ".zip");

                    using (var zipArchive = new ZipArchive(System.IO.File.OpenWrite(dbPath), ZipArchiveMode.Create))
                        using (var entry = zipArchive.CreateEntry(file.FileName).Open())
                            file.CopyTo(entry);
                }

                return dbPath;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetSanitarizedUniqueActualName(IFormFile file)
        {
            string actualName = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition)
                        .FileName
                        .Trim('"');
            actualName = SanitarizeFileNameAndRemoveExtension(actualName);
            return FileUploadService.AddMd5Suffix(actualName);
        }

        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
