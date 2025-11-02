using System.IO;
using System.Threading.Tasks;
using temsAPI.Data.Entities.Report;

namespace temsAPI.Services.Report
{
    public class ReportingService
    {
        ReportDataGenerator _reportDataGenerator;
        IdentityService _identityService;

        public ReportingService(
            ReportDataGenerator reportDataGenerator,
            IdentityService identityService)
        {
            _reportDataGenerator = reportDataGenerator;
            _identityService = identityService;
        }

        public async Task<FileInfo> GenerateReport(ReportTemplate template, string filePath)
        {
            var generatedBy = (await _identityService.GetCurrentUserAsync()).Identifier;
            var reportData = await _reportDataGenerator.GenerateReportData(template, generatedBy);
            var reportGenerator = new ReportGenerator.Services.ReportGenerator(filePath);
            return reportGenerator.GenerateReport(reportData);
        }
    }
}
