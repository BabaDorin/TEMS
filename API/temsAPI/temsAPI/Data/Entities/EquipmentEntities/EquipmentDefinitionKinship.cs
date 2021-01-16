using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(ParentDefinitionID))]
    public class EquipmentDefinitionKinship
    {
        [Key]
        public string ID { get; set; }

        [ForeignKey("ParentDefinitionID")]
        public EquipmentDefinition ParentDefinition { get; set; }
        public string ParentDefinitionID { get; set; }


        [ForeignKey("ChildDefinitionID")]
        public EquipmentDefinition ChildDefinition { get; set; }
        public string ChildDefinitionID { get; set; }
    }
}
