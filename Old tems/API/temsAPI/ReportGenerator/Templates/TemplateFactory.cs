using ReportGenerator.Models;
using System.IO;

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
