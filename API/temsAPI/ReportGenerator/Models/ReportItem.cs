using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models
{
    public class ReportItem
    {
        public string Identifier { get; set; }
        public List<ReportItemField> Fields { get; set; }
    }
}
