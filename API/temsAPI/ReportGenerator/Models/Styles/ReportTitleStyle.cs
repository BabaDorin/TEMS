using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models.Styles
{
    class ReportTitleStyle : IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("ReportTitleStyle");
            namedStyle.Style.Font.Size = 18;
            namedStyle.Style.Font.Bold = true;
            namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }
}
