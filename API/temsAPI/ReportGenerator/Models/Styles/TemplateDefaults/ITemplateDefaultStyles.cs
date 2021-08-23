using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;

namespace ReportGenerator.Models.Styles.TemplateDefaults
{
    interface ITemplateDefaultStyles
    {
        public ExcelNamedStyleXml GetStyleWithDefaultsSet(ExcelPackage pck, string styleName);
    }
}
