using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class PropertyEquipmentTypeAssociation
    {
        // Which properties belong to which equipment types

        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [ForeignKey("TypeID")]
        public EquipmentType Type { get; set; }

        [MaxLength(150)]
        public string TypeID { get; set; }

        [ForeignKey("PropertyID")]
        public Property Property { get; set; }

        [MaxLength(150)]
        public string PropertyID { get; set; }
    }
}
