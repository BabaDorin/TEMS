using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class DataType
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Property> DataTypeProperties { get; set; }

        public Type GetNativeType()
        {
            switch (Name.ToLower())
            {
                case "number": return typeof(double);
                case "boolean": return typeof(bool);
                default: return typeof(string);
            }
        }
    }
}
