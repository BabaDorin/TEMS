using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class EquipmentTypeKinship
    {
        [ForeignKey("ParentId")]
        public EquipmentType Parent { get; set; }
        public string ParentId { get; set; }

        [ForeignKey("ChildId")]
        public EquipmentType Child { get; set; }
        public string ChildId { get; set; }
    }
}
