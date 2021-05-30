using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models.Styles
{
    class BorderedStyle : IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("Bordered");
            namedStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            namedStyle.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            namedStyle.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            namedStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }
    }
}
