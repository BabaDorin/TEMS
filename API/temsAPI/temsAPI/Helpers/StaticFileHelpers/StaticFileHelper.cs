using System;
using System.Collections.Generic;
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
    }
}
