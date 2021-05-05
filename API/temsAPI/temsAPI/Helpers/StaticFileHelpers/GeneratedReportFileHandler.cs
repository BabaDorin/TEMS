using System;
using System.IO;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public class GeneratedReportFileHandler : StaticFileHandler
    {
        public override string FolderPath => Path.Combine("StaticFiles", "GeneratedReports");

        public string GetDBPath()
        {
            string f = FolderPath + "\\Report_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
            if (File.Exists(f))
                f = Path.GetFileNameWithoutExtension(f) + Guid.NewGuid().ToString() + ".xlsx";
            
            return f;
        }
    }
}
