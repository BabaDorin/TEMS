using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class ReportTitleStyle : IWorkBookStyle
    {
        public string StyleName => "ReportTitleStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            namedStyle.Style.Font.Size = 18;
            namedStyle.Style.Font.Bold = true;
            namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }
}
