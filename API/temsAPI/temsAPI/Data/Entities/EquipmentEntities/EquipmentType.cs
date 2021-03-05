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
        public string Id { get; set; }
        public string Type { get; set; }
        public bool IsArchieved { get; set; }

        //public virtual ICollection<PropertyEquipmentTypeAssociation> PropertyEquipmentTypeAssociations { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
        public virtual ICollection<EquipmentDefinition> EquipmentDefinitions { get; set; }
        public virtual ICollection<EquipmentTypeKinship> EquipmentTypeKinships { get; set; }

        public EquipmentType()
        {
            //PropertyEquipmentTypeAssociations = new List<PropertyEquipmentTypeAssociation>();
            Properties = new List<Property>();
            EquipmentDefinitions = new List<EquipmentDefinition>();
            EquipmentTypeKinships = new List<EquipmentTypeKinship>();
        }
    }
}
