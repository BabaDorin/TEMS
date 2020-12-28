using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities
{
    public class EquipmentDefinition
    {
        [Key]
        public string ID { get; set; }

        public string Identifier { get; set; }

        [ForeignKey("EquipmentTypeID")]
        public EquipmentType EquipmentType { get; set; }
        public string EquipmentTypeID { get; set; }

    }
}
