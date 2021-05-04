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
            string uniqueSuffix = Guid.NewGuid().ToString();
            var file = new FileInfo(@$"C:\Users\Dorin\Desktop\testreport{uniqueSuffix}.xlsx");
            SaveExcelFile(reportData, file);
            return file;
        }

        private void SaveExcelFile(ReportData reportData, FileInfo file)
        {
            if (file.Exists) file.Delete();
            
            using (var pck = new ExcelPackage(file))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Items");
                int lastStartingLine = 1;
                for (int i = 0; i < reportData.ReportItemGroups.Count; i++)
                {
                    var itemGroup = reportData.ReportItemGroups[i];
                    ws.Cells[$"A{lastStartingLine}"].LoadFromDataTable(itemGroup.ItemsTable, true);
                    lastStartingLine = lastStartingLine + itemGroup.ItemsTable.Rows.Count + 3;
                    pck.Save();
                }
            }
        }
    }
}
