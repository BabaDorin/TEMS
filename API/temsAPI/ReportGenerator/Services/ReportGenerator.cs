using OfficeOpenXml;
using ReportGenerator.Models;
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
        public ReportGenerator()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public FileInfo GenerateReport(ReportData reportData)
        {
            // Logic of creating an excel file, based on provided ReportData
            var file = new FileInfo(@"C:\Users\Dorin\Desktop\testreport2.xlsx");
            SaveExcelFile(reportData, file);
            return file;
        }

        private void SaveExcelFile(ReportData reportData, FileInfo file)
        {
            if (file.Exists) file.Delete();
            
            using (var pck = new ExcelPackage(file))
            {
                foreach (ReportItemGroup itemGroup in reportData.ReportItemGroups)
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Items");
                    ws.Cells["A1"].LoadFromDataTable(itemGroup.ItemsTable, true);
                    pck.Save();
                }
            }
        }
    }
}
