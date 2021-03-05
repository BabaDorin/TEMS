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
        [Key]
        public string Id { get; set; }

        [ForeignKey("ParentEquipmentTypeId")]
        public EquipmentType ParentEquipmentType { get; set; }
        public string ParentEquipmentTypeId { get; set; }

        [ForeignKey("ChildEquipmentTypeId")]
        public EquipmentType ChildEquipmentType { get; set; }
        public string ChildEquipmentTypeId { get; set; }
    }
}
