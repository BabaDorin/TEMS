using OfficeOpenXml;
using ReportGenerator.Models;
using ReportGenerator.Models.Styles;
using ReportGenerator.Templates;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator.Services
{
    public class ReportGenerator
    {
        private string _filePath = null;
        public ReportGenerator(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _filePath = filePath;
        }

        public FileInfo GenerateReport(ReportData reportData, SICReportTemplate template = SICReportTemplate.Default)
        {
            // Logic of creating an excel file, based on provided ReportData
            var file = new FileInfo(_filePath);
            if (file.Exists) file.Delete();

            var reportTemplate = new TemplateFactory().GetTemplate(template, reportData, file);
            reportTemplate.GenerateReport();

            return file;
        }

        private void SaveExcelFile(ReportData reportData, FileInfo file)
        {
            
            
            
        }
    }
}
