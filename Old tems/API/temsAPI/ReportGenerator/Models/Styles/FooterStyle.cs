using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class FooterStyle : IWorkBookStyle
    {
        public string StyleName => "FooterStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            namedStyle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            namedStyle.Style.WrapText = true;
        }
    }
}
