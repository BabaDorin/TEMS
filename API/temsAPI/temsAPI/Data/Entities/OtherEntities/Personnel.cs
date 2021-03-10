using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.KeyEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Personnel
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
#nullable enable
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }

        public string? ImagePath { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }

        public virtual ICollection<PersonnelEquipmentAllocation> PersonnelEquipmentAllocations { get; set; } = new List<PersonnelEquipmentAllocation>();
        public virtual ICollection<PersonnelRoomSupervisory> PersonnelRoomSupervisories { get; set; } = new List<PersonnelRoomSupervisory>();
        public virtual ICollection<KeyAllocation> KeyAllocations { get; set; } = new List<KeyAllocation>();
        public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        // BEFREE: Add multiple Emails and Phone numbers support later if needed.
    }
}
