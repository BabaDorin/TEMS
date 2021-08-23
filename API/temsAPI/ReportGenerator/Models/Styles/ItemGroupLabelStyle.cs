using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models.Styles
{
    class ItemGroupLabelStyle : IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("ItemGroupLabelStyle");
            namedStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        }
    }
}
