using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class HeaderStyle : IWorkBookStyle
    {
        public string StyleName => "HeaderStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            namedStyle.Style.WrapText = true;
            namedStyle.Style.Font.Bold = true;
        }
    }
}
