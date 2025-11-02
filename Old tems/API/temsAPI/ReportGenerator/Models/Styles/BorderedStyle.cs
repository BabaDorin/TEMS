using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class BorderedStyle : IWorkBookStyle
    {
        public string StyleName { get; } = "BorderedStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            namedStyle.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            namedStyle.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            namedStyle.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            namedStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }
    }
}
