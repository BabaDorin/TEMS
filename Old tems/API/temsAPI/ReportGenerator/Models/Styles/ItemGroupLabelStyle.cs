using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class ItemGroupLabelStyle : IWorkBookStyle
    {
        public string StyleName => "ItemGroupLabelStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            namedStyle.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            namedStyle.Style.WrapText = true;
            namedStyle.Style.Font.Bold = true;
        }
    }
}
