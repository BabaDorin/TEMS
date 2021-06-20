using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public enum StaticFileHandlers
    {
        LibraryItem,
        GeneratedReport
    }

    public static class StaticFileHelper
    {
        public static StaticFileHandler GetFileHandler(StaticFileHandlers type)
        {
            switch (type)
            {
                case StaticFileHandlers.GeneratedReport: return new GeneratedReportFileHandler();
                case StaticFileHandlers.LibraryItem: return new LibraryItemFileHandler();
                default: throw new Exception("There is no such file handler");
            }
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
    }
}
