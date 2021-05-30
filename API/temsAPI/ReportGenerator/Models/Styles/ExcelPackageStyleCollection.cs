using OfficeOpenXml;
using OfficeOpenXml.Style.XmlAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models.Styles
{
    // Wiki:
    // Add new style:
    // 1. Add new enum for ExcelStyleNames
    // 2. Create style class which implements IWorkBookStyle
    // 3. Create the style logic inside the class. 
    //    The name of style should be the enum name (Also, it's case sensitive)
    // 4. Add the newly created style into the list of styles, which is located inside the
    //    constructor.

    public enum ExcelStyleNames
    {
        PrimaryHeader,
        SecondaryHeader,
        TertiaryHeader,
        Bordered
    }

    class ExcelPackageStyleCollection
    {
        private ExcelPackage _pck;

        public string GetStyleName(ExcelStyleNames styleName) => styleName.ToString();

        public ExcelPackageStyleCollection(ExcelPackage pck)
        {
            _pck = pck;

            List<IWorkBookStyle> styles = new List<IWorkBookStyle>()
            {
                new PrimaryHeader(),
                new SecondaryHeader(),
                new BorderedStyle(),
                new TertiaryHeader()
            };

            foreach (var style in styles)
            {
                style.CreateStyle(pck);
            }
        }
    }
}
