using System;
using System.Collections.Generic;
using System.Text;

namespace ReportGenerator.Models
{
    public class ReportItemGroup
    {
        public string Name { get; set; }
        List<ReportItem> Items { get; set; }
        public List<string> ReportItemGroupSignatories { get; set; }
    }
}
