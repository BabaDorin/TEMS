using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    class SignatoryStyle : IWorkBookStyle
    {
        public string StyleName => "SignatoryStyle";

        public void AppendStyles(ExcelNamedStyleXml namedStyle)
        {
            namedStyle.Style.ShrinkToFit = true;
        }
    }
}
