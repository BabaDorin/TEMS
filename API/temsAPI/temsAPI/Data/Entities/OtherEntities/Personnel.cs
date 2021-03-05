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

        public virtual ICollection<PersonnelEquipmentAllocation> PersonnelEquipmentAllocations { get; set; }
        public virtual ICollection<PersonnelRoomSupervisory> PersonnelRoomSupervisories { get; set; }
        public virtual ICollection<KeyAllocation> KeyAllocations { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        // BEFREE: Add multiple Emails and Phone numbers support later if needed.
    }
}
