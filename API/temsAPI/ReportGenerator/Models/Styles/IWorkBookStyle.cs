using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models.Styles
{
    interface IWorkBookStyle
    {
        public void CreateStyle(ExcelPackage pck);
    }
}
