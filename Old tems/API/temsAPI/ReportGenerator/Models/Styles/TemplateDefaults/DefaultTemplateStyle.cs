using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ReportGenerator.Models.Styles.TemplateDefaults
{
    class DefaultTemplateStyle : ITemplateDefaultStyles
    {
        public ExcelNamedStyleXml GetStyleWithDefaultsSet(ExcelPackage pck, string styleName)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle(styleName);

            // Set style defaults
            namedStyle.Style.Font.SetFromFont(new Font("Times New Roman", 12));
            namedStyle.Style.Font.Size = 12;

            return namedStyle;
        }
    }
}
