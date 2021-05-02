using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Services.Report
{
    public class ReportingService
    {
        IUnitOfWork _unitOfWork;
        UserManager<TEMSUser> _userManager;

        public ReportingService(IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<FileInfo> GenerateReport(ReportTemplate template)
        {
            var reportDataGenerator = new ReportDataGenerator(_unitOfWork, _userManager);
            var reportData = await reportDataGenerator.GenerateReportData(template);
            var reportGenerator = new ReportGenerator.Services.ReportGenerator();
            return reportGenerator.GenerateReport(reportData);
        }
    }
}
