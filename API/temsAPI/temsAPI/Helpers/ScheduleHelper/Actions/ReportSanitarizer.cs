using Microsoft.AspNetCore.Razor.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;
using temsAPI.Helpers.StaticFileHelpers;

namespace temsAPI.Helpers.ScheduleHelper.Actions
{
    public class ReportSanitarizer : IScheduledAction
    {
        IUnitOfWork _unitOfWork;
        GeneratedReportFileHandler fileHandler = new();

        public ReportSanitarizer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Removes files that do not have any reports associated and vice versa.
        /// Also checks if there are not any foreign files inside the reports folder
        /// </summary>
        public async Task Start()
        {
            var reports = (await _unitOfWork.Reports.FindAll<Report>()).ToList();

            List<Report> toBeRemoved = new List<Report>();
            foreach (var report in reports)
            {
                if (!fileHandler.FileExists(report.DBPath))
                {
                    toBeRemoved.Add(report);
                    continue;
                }
            }

            if(toBeRemoved.Count > 0)
            {
                foreach (var report in toBeRemoved)
                    _unitOfWork.Reports.Delete(report);

                await _unitOfWork.Save();
            }
            
            var files = Directory.GetFiles(fileHandler.FolderPath);
            foreach(var file in files)
            {
                if(Path.GetExtension(file) != ".xlsx"
                    || !reports.Any(qu => qu.DBPath == file))
                {
                    fileHandler.DeleteFile(file);
                }
            }
        }
    }
}
