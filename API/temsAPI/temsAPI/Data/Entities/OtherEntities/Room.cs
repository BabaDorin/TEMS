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
        public string Id { get; set; }

        public string Identifier { get; set; }
#nullable enable
        public string? Description { get; set; }
        public int? Floor { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }

        public virtual ICollection<RoomEquipmentAllocation> RoomEquipmentAllocations { get; set; } = new List<RoomEquipmentAllocation>();
        public virtual ICollection<PersonnelRoomSupervisory> PersonnelRoomSupervisories { get; set; } = new List<PersonnelRoomSupervisory>();
        public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
        public virtual ICollection<Ticket> Tickets  { get; set; } = new List<Ticket>();
    }
}
