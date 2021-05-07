using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;
using System.Drawing;

namespace ReportGenerator.Models.Styles
{
    internal class SimpleHeaderStyle : IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck)
        {
            var namedStyle = pck.Workbook.Styles.CreateNamedStyle("SimpleHeader");
            namedStyle.Style.Font.UnderLine = true;
            namedStyle.Style.Font.Color.SetColor(Color.Blue);
        }
    }
}
