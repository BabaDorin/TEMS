using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class ItemGroupTableContentStyle : IWorkBookStyle
    {
        public string StyleName => "ItemGroupTableContentStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            new BorderedStyle().AppendStyles(namedStyle);

            namedStyle.Style.WrapText = true;
            namedStyle.Style.Font.Size = 9;
        }
    }
}
