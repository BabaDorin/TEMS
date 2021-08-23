namespace ReportGenerator.Models
{
    public enum ReportItemFieldDataType
    {
        Number, Boolean, Text
    }

    public class ReportItemField
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ReportItemFieldDataType DataType { get; set; }
    }
}
