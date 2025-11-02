using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;
using System.Drawing;

namespace ReportGenerator.Models.Styles
{
    class ItemGroupTableHeaderStyle : IWorkBookStyle
    {
        public string StyleName => "ItemGroupTableHeaderStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            new BorderedStyle().AppendStyles(namedStyle);

            namedStyle.Style.Fill.SetBackground(Color.FromArgb(218, 224, 235));
            namedStyle.Style.Font.Bold = true;
            namedStyle.Style.Font.Size = 9;
        }
    }
}
