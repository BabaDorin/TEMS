namespace SIC_Parser.Models
{

    // BEFREE: Private set for TEMSSerialNumber !??
    public interface ITEMSIndexable
    {
        // The index that uniquely identifies an equipment
        string TEMSSerialNumber { get; }

        // IdentifierPropertyName (ex: For RAM chips, PartNumber is the property that identifies the "model" of the ram)
        // This will be treated as equipment's definition name
        string IdentifierPropertyName { get; }

        void BuildIndex(object parameter);
    }
}
