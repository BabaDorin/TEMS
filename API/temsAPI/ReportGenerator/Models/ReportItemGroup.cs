using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ReportGenerator.Models
{
    public class ReportItemGroup
    {
        public string Name { get; set; }
        public DataTable ItemsTable { get; set; }
        public List<string> ReportItemGroupSignatories { get; set; }
    }
}
