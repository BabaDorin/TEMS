using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Entities.UserEntities
{
    public class TEMSUser : IdentityUser
    {
        public bool IsArchieved { get; set; }

        public virtual ICollection<Announcement> Announcements { get; set; }
        public virtual ICollection<Ticket> ClosedTickets { get; set; }
        public virtual ICollection<Equipment> RegisteredEquipments { get; set; }
    }
}
