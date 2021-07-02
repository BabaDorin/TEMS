using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class EquipmentTypeKinship
    {
        [ForeignKey("ParentId")]
        public EquipmentType Parent { get; set; }

        [MaxLength(150)]
        public string ParentId { get; set; }

        [ForeignKey("ChildId")]
        public EquipmentType Child { get; set; }

        [MaxLength(150)]
        public string ChildId { get; set; }
    }
}
