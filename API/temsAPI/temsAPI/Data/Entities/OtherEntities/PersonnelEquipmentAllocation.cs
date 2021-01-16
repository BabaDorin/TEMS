using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class PersonnelEquipmentAllocation
    {
        [Key]
        public string ID { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment Equipment { get; set; }
        public string EquipmentID { get; set; }

        [ForeignKey("PersonnelID")]
        public Personnel Personnel { get; set; }
        public string PersonnelID { get; set; }

        public DateTime DateAllocated { get; set; }
        public DateTime? DateReturned { get; set; }
    }
}
