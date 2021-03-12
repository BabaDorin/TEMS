using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class EquipmentAllocation
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment Equipment { get; set; }
        public string EquipmentID { get; set; }
        public DateTime DateAllocated { get; set; }
        public bool IsArchieved { get; set; }

#nullable enable
        [ForeignKey("PersonnelID")]
        public Personnel? Personnel { get; set; }
        public string? PersonnelID { get; set; }

        [ForeignKey("RoomID")]
        public Room? Room { get; set; }
        public string? RoomID { get; set; }

        public DateTime? DateReturned { get; set; }
#nullable disable
    }
}
