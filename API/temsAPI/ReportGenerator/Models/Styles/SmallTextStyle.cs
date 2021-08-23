using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models.Styles
{
    class SmallTextStyle : IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("SmallTextStyle");
            namedStyle.Style.Font.Size = 8;
        }
    }
}
