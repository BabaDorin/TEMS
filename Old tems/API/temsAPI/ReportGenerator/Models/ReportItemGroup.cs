using System.Data;

namespace ReportGenerator.Models
{
    public class ReportItemGroup
    {
        public string Name { get; set; }
        public DataTable ItemsTable { get; set; }

        // Useful feature, might be implemented in the future.
        //public List<string> ReportItemGroupSignatories { get; set; } 
    }
}
