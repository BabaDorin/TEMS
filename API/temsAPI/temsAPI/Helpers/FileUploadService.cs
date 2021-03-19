using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            string sanitarizedFileName = SanitarizeFileNameAndRemoveExtension(fileName);
            return sanitarizedFileName += "_" + EncryptionService.Md5(DateTime.Now.ToString());
        }

        public static bool CompressAndSave(IFormFile file, string actualName)
        {
            try
            {
                if (file.Length > 0)
                {
                    var fullPath = Path.Combine(FileUploadService.PathToSave, actualName);
                    var dbPath = Path.Combine(FileUploadService.LibraryFolderName, actualName);

                    using (var zipArchive = new ZipArchive(System.IO.File.OpenWrite(dbPath + ".zip"), ZipArchiveMode.Create))
                    {
                        using (var entry = zipArchive.CreateEntry(file.FileName).Open())
                        {
                            file.CopyTo(entry);
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
