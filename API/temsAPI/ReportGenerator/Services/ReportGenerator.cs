using OfficeOpenXml;
using ReportGenerator.Models;
using ReportGenerator.Models.Styles;
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

        public FileInfo GenerateReport(ReportData reportData)
        {
            // Logic of creating an excel file, based on provided ReportData
            var file = new FileInfo(_filePath);
            SaveExcelFile(reportData, file);
            return file;
        }

        private void SaveExcelFile(ReportData reportData, FileInfo file)
        {
            if (file.Exists) file.Delete();
            
            using (var pck = new ExcelPackage(file))
            {
                var styles = new ExcelPackageStyleCollection(pck);
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(reportData.Name ?? "Items");

                for (int i = 2; i < 20; i++)
                    ws.Column(i).Width = 13;
                
                // Used for tracking the last written row
                int lastRowTracker = 2;

                // Write report header (if any)
                if(reportData.Header != null)
                {
                    ws.Cells[$"B{lastRowTracker}"].StyleName = styles.GetStyleName(ExcelStyleNames.PrimaryHeader);
                    ws.Cells[$"B{lastRowTracker}"].Value = reportData.Header;
                    lastRowTracker += 3;
                }

                // Write report data
                for (int i = 0; i < reportData.ReportItemGroups.Count; i++)
                {
                    ReportItemGroup itemGroup = reportData.ReportItemGroups[i];

                    // Write report item group name (delimitator)
                    ws.Cells[$"B{lastRowTracker}"].Value = "🔖 " + itemGroup.Name;
                    ws.Cells[$"B{lastRowTracker}"].StyleName = styles.GetStyleName(ExcelStyleNames.SecondaryHeader);
                    lastRowTracker += 2;

                    // Write item group data, represented by a data table
                    var writtenCells = ws.Cells[$"B{lastRowTracker}"].LoadFromDataTable(itemGroup.ItemsTable, true);
                    
                    // Table styling
                    writtenCells.StyleName = styles.GetStyleName(ExcelStyleNames.Bordered);
                    writtenCells.Style.WrapText = true;

                    lastRowTracker += itemGroup.ItemsTable.Rows.Count + 3;
                }

                // Write report footer (if any)
                if (reportData.Footer != null)
                {
                    ws.Cells[$"B{lastRowTracker}"].StyleName = styles.GetStyleName(ExcelStyleNames.SecondaryHeader);
                    ws.Cells[$"B{lastRowTracker}"].Value = reportData.Footer;

                    lastRowTracker += 3;
                }

                if(reportData.Signatories != null && reportData.Signatories.Count > 0)
                {
                    ws.Cells[$"B{lastRowTracker}"].StyleName = styles.GetStyleName(ExcelStyleNames.SecondaryHeader);
                    ws.Cells[$"B{lastRowTracker}"].Value = "Signatories list";
                    lastRowTracker += 2;

                    foreach (string s in reportData.Signatories)
                    {
                        ws.Cells[$"B{lastRowTracker}"].StyleName = styles.GetStyleName(ExcelStyleNames.TertiaryHeader);
                        ws.Cells[$"B{lastRowTracker}"].Value = s;
                        ws.Cells[$"B{++lastRowTracker}"].Value = "________________________ (Signature)";

                        lastRowTracker++;
                    }
                }

                pck.Save();
            }
        }
    }
}
