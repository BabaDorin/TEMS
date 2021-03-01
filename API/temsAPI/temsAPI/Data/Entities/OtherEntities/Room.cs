using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Room
    {
        [Key]
        public string ID { get; set; }

        public string Identifier { get; set; }
#nullable enable
        public string? Description { get; set; }
        public int? Floor { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }

        public virtual ICollection<RoomEquipmentAllocation> RoomEquipmentAllocations { get; set; }
        public virtual ICollection<PersonnelRoomSupervisory> PersonnelRoomSupervisories { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Ticket> Tickets  { get; set; }
    }
}
