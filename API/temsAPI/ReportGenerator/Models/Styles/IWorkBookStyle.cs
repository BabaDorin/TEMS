using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles
{
    interface IWorkBookStyle
    {
        public string StyleName { get; }

        public void AppendStyles(ExcelNamedStyleXml namedStyle);
    }
}
