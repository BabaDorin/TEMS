using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.UserEntities
{
    public class TEMSUser : IdentityUser
    {
        public bool IsArchieved { get; set; }

#nullable enable
        [ForeignKey("PersonnelId")]
        public Personnel? Personnel { get; set; }
        public string? PersonnelId { get; set; }
#nullable disable

        public virtual ICollection<Announcement> Announcements { get; set; }
        public virtual ICollection<Ticket> ClosedTickets { get; set; }
        public virtual ICollection<Ticket> AssignedTickets { get; set; }
        public virtual ICollection<Ticket> CreatedTickets { get; set; }
        public virtual ICollection<Equipment> RegisteredEquipments { get; set; }
    }
}
