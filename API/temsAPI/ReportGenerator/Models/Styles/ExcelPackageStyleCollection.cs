using OfficeOpenXml;
using ReportGenerator.Models.Styles.TemplateDefaults;
using System.Collections.Generic;

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
        ReportTitleStyle,
        HeaderStyle,
        FooterStyle,
        BorderedStyle,
        SmallTextStyle,
        ItemGroupLabelStyle,
        ItemGroupTableHeaderStyle,
        ItemGroupTableContentStyle,
        SignatoryStyle,
        UnderlineStyle
    }

    class ExcelPackageStyleCollection
    {
        public string GetStyleName(ExcelStyleNames styleName) => styleName.ToString();

        public ExcelPackageStyleCollection(ExcelPackage pck, ITemplateDefaultStyles templateDefaultStyles)
        {
            List<IWorkBookStyle> styles = new List<IWorkBookStyle>()
            {
                new HeaderStyle(),
                new BorderedStyle(),
                new ReportTitleStyle(),
                new SmallTextStyle(),
                new ItemGroupLabelStyle(),
                new ItemGroupTableHeaderStyle(),
                new ItemGroupTableContentStyle(),
                new FooterStyle(),
                new SignatoryStyle(),
                new UnderlineStyle()
            };

            foreach (var style in styles)
            {
                var defaultNamedStyle = templateDefaultStyles.GetStyleWithDefaultsSet(pck, style.StyleName);
                style.AppendStyles(defaultNamedStyle);
            }
        }
    }
}
