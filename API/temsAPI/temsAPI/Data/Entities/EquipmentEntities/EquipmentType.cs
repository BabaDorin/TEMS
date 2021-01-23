using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class EquipmentType
    {
        [Key]
        public string ID { get; set; }

        public string Type { get; set; }

#nullable enable
        [ForeignKey("ParentEquipmentTypeID")]
        public EquipmentType? ParentEquipmentType { get; set; }
        public string? ParentEquipmentTypeID { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }

        public virtual ICollection<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; set; }
        public virtual ICollection<EquipmentDefinition> EquipmentDefinitions { get; set; }
    }
}
