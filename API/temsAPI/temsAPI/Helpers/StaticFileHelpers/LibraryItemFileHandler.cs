using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public class LibraryItemFileHandler : StaticFileHandler
    {
        public override string FolderPath
        {
            get => Path.Combine("StaticFiles", "LibraryUploads");
        }

        public string CompressAndSave(IFormFile file, string actualName)
        {
            try
            {
                string dbPath = null;
                if (file.Length > 0)
                {
                    dbPath = String.Concat(
                        Path.Combine(FolderPath, actualName),
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

        public string GetSanitarizedUniqueActualName(IFormFile file)
        {
            string actualName = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition)
                        .FileName
                        .Trim('"');
            actualName = SanitarizeFileNameAndRemoveExtension(actualName);
            return AddMd5Suffix(actualName);
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
    }
}
