using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReportGenerator.Templates
{
    class TemplateFactory
    {
        public IReportTemplate GetTemplate(SICReportTemplate template, ReportData reportData, FileInfo file)
        {
            switch (template)
            {
                default: return new DefaultReportTemplate(reportData, file);
            }
        }
    }
}
