using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class RoomEquipmentAllocation
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment Equipment { get; set; }
        public string EquipmentID { get; set; }

        [ForeignKey("RoomID")]
        public Room Room { get; set; }
        public string RoomID { get; set; }

        public DateTime DateAllocated { get; set; }
#nullable enable
        public DateTime? DateReturned { get; set; }
#nullable disable

    }
}
