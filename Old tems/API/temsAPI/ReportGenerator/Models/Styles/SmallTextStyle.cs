using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class SmallTextStyle : IWorkBookStyle
    {
        public string StyleName => "SmallTextStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            namedStyle.Style.Font.Size = 9;
        }
    }
}
