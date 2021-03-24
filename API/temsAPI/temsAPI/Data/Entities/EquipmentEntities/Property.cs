using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class Property
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; } = false;
        public bool IsArchieved { get; set; }

#nullable enable
        public string? Description { get; set; }
        public string? DisplayName { get; set; }

        public int? Min { get; set; }
        public int? Max { get; set; }
        public string? Options { get; set; } // Will be integrated soon, i guess

        //[ForeignKey("DataTypeID")]
        public DataType? DataType { get; set; }
        public string? DataTypeID { get; set; }
#nullable disable

        //public virtual ICollection<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; set; }
        public virtual ICollection<EquipmentType> EquipmentTypes { get; set; }
        public virtual ICollection<EquipmentSpecifications> EquipmentSpecifications { get; set; }
    }
}
