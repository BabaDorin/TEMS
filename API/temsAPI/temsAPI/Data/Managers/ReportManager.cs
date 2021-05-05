using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.System_Files;

namespace temsAPI.Data.Managers
{
    public class ReportManager
    {
        private IUnitOfWork _unitOfWork;
        GeneratedReportFileHandler fileHandler = new();
        AppSettings _appSettings;

        public ReportManager(
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
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

        public async Task CheckForReportsOverflow()
        {
            int totalReports = await _unitOfWork.Reports.Count();
            while (totalReports >= _appSettings.MaxGeneratedReportsStored)
            {
                var toBeRemoved = (await _unitOfWork.Reports
                    .FindAll<Report>(
                        orderBy: q => q.OrderBy(q => q.DateGenerated)
                    )).First();
                string result = await RemoveReport(toBeRemoved);

                if (result == null)
                    --totalReports;
                else
                    break;
            }
        }

        public async Task<MemoryStream> GetReportMemoryStream(string reportDBPath)
        {
            if (!System.IO.File.Exists(reportDBPath))
                return null;

            var memory = new MemoryStream();
            await using (var stream = new FileStream(reportDBPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return memory;
        }
    }
}
