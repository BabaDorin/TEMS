using System;
using System.IO;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public class GeneratedReportFileHandler : StaticFileHandler
    {
        public override string FolderPath => Path.Combine("StaticFiles", "GeneratedReports");
        public string TempReportFolderPath => Path.Combine("StaticFiles", "TempReports");

        public string GetDBPath()
        {
            return DBPathBuilder(FolderPath);
        }

        public string GetTempDBPath()
        {
            return DBPathBuilder(TempReportFolderPath);
        }

        private string DBPathBuilder(string folderPath)
        {
            string f = folderPath + "\\Report_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx";
            if (File.Exists(f))
                f = Path.GetFileNameWithoutExtension(f) + Guid.NewGuid().ToString() + ".xlsx";

            return f;
        }
    }
}
