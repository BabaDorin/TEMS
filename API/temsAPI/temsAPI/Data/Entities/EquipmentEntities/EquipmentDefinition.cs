using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(Identifier))]
    public class EquipmentDefinition
    {
        [Key]
        public string Id { get; set; }

        public string Identifier { get; set; }

#nullable enable
        [ForeignKey("EquipmentTypeID")]
        public EquipmentType? EquipmentType { get; set; }
        public string? EquipmentTypeID { get; set; }

        [ForeignKey("ParentID")]
        public EquipmentDefinition? Parent { get; set; }
        public string? ParentID { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }
        public virtual ICollection<EquipmentSpecifications> EquipmentSpecifications { get; set; }
        public virtual ICollection<EquipmentDefinition> Children { get; set; }

    }
}
