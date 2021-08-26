using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class DataType
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public virtual ICollection<Property> DataTypeProperties { get; set; }

        public Type GetNativeType()
        {
            switch (Name.ToLower())
            {
                case "number": return typeof(double);
                case "bool": return typeof(bool);
                default: return typeof(string);
            }
        }

        public bool TryParseValue(dynamic value)
        {
            switch (Name.ToLower())
            {
                case "number": return Double.TryParse(value.ToString(), out double doubleVal);
                case "bool": return Boolean.TryParse(value.ToString(), out bool boolValue);
                default: return true;
            }
        }
    }
}
