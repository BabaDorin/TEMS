using System.IO;
using System.Threading.Tasks;
using temsAPI.Data.Entities.Report;

namespace temsAPI.Services.Report
{
    public class ReportingService
    {
        ReportDataGenerator _reportDataGenerator;

        public ReportingService(ReportDataGenerator reportDataGenerator)
        {
            _reportDataGenerator = reportDataGenerator;
        }

        public async Task<FileInfo> GenerateReport(ReportTemplate template, string filePath)
        {
            var reportData = await _reportDataGenerator.GenerateReportData(template);
            var reportGenerator = new ReportGenerator.Services.ReportGenerator(filePath);
            return reportGenerator.GenerateReport(reportData);
        }
    }
}
