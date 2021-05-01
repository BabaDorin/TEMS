using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReportGenerator.Services
{
    public class ReportGenerator
    {
        public FileInfo GenerateReport(ReportData reportData)
        {
            // Logic of creating an excel file, based on provided ReportData

            return new FileInfo("testreport.xlms");
        }
    }
}
