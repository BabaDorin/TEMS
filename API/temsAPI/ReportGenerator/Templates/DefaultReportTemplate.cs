using OfficeOpenXml;
using ReportGenerator.Models;
using ReportGenerator.Models.Styles;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace ReportGenerator.Templates
{
    class DefaultReportTemplate : IReportTemplate
    {
        readonly ReportData _reportData;
        readonly FileInfo _file;

        public DefaultReportTemplate(ReportData reportData, FileInfo file)
        {
            _reportData = reportData;
            _file = file;
        }

        public void GenerateReport()
        {
            int columnWidth = 13;

            using (var pck = new ExcelPackage(_file))
            {
                var styles = new ExcelPackageStyleCollection(pck);
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(_reportData.Name ?? "Items");

                ws.Column(1).Width = 15;

                for (int i = 2; i < 20; i++)
                    ws.Column(i).Width = columnWidth;

                // Used for tracking the last written row
                int lastRowTracker = 2;

                // Write report header (if any)
                if (_reportData.Header != null)
                {
                    ws.Cells[$"B{lastRowTracker}"].StyleName = styles.GetStyleName(ExcelStyleNames.PrimaryHeader);
                    ws.Cells[$"B{lastRowTracker}"].Value = _reportData.Header;
                    lastRowTracker += 4;
                }

                // Add TEMS and CIHCahul logos
                string resourcesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Resources");

                string temsLogoPath = Path.Combine(resourcesPath, "tems_logo_small.png");
                if(File.Exists(temsLogoPath))
                    using (Image image = Image.FromFile(temsLogoPath))
                    {
                        var excelImage = ws.Drawings.AddPicture("TEMS", image);
                        excelImage.SetPosition(lastRowTracker, 0, 1, 0);
                    }

                string cihcLogoPath = Path.Combine(resourcesPath, "cihc_logo_small.jpg");
                if (File.Exists(cihcLogoPath))
                    using (Image image = Image.FromFile(cihcLogoPath))
                    {
                        var excelImage = ws.Drawings.AddPicture("CIHC", image);
                        excelImage.SetPosition(lastRowTracker, 0, 3, 0);
                    }

                lastRowTracker +=4;

                // Write report data
                for (int i = 0; i < _reportData.ReportItemGroups.Count; i++)
                {
                    ReportItemGroup itemGroup = _reportData.ReportItemGroups[i];

                    // Write report item group name (delimitator)
                    ws.Cells[$"B{lastRowTracker}"].Value = itemGroup.Name == null ? "" : "🔖 " + itemGroup.Name;
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
                if (_reportData.Footer != null)
                {
                    ws.Cells[$"B{lastRowTracker}"].StyleName = styles.GetStyleName(ExcelStyleNames.SecondaryHeader);
                    ws.Cells[$"B{lastRowTracker}"].Value = _reportData.Footer;

                    lastRowTracker += 3;
                }

                if (_reportData.Signatories != null && _reportData.Signatories.Count > 0)
                {
                    ws.Cells[$"B{lastRowTracker}"].StyleName = styles.GetStyleName(ExcelStyleNames.SecondaryHeader);
                    ws.Cells[$"B{lastRowTracker}"].Value = "Signatories list";
                    lastRowTracker += 2;

                    foreach (string s in _reportData.Signatories)
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
