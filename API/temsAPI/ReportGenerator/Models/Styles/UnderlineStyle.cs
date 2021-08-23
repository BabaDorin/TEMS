using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class UnderlineStyle : IWorkBookStyle
    {
        public string StyleName => "UnderlineStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            namedStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }
    }
}
