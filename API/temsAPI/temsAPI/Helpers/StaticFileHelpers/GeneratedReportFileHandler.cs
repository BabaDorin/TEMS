using System.IO;

namespace temsAPI.Helpers.StaticFileHelpers
{
    public class GeneratedReportFileHandler : StaticFileHandler
    {
        public override string FolderPath => Path.Combine("StaticFiles", "GeneratedReports");
    }
}
