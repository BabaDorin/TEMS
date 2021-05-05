using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;
using temsAPI.Helpers.StaticFileHelpers;

namespace temsAPI.Data.Managers
{
    public class ReportManager
    {
        private IUnitOfWork _unitOfWork;
        GeneratedReportFileHandler fileHandler = new();

        public ReportManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Report> GetReport(string reportId)
        {
            return (await _unitOfWork.Reports
                .Find<Report>(q => q.Id == reportId))
                .FirstOrDefault();
        }

        public async Task<string> RemoveReport(Report report)
        {
            try
            {
                _unitOfWork.Reports.Delete(report);
                fileHandler.DeleteFile(report.DBPath);
                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
