using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models.Styles
{
    class PrimaryHeader : IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("PrimaryHeader");
            namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }

    class SecondaryHeader : IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("SecondaryHeader");
            namedStyle.Style.Font.Bold = true;
            namedStyle.Style.Font.Size = 16;
        }
    }

    class TertiaryHeader : IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("TertiaryHeader");
            namedStyle.Style.Font.Bold = true;
            namedStyle.Style.Font.Size = 12;
        }
    }
}
