using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.UserEntities
{
    public class TEMSUser : IdentityUser, IArchiveable, IIdentifiable
    {
        public DateTime DateRegistered { get; set; }
        public DateTime DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get { return isArchieved; }
            set { isArchieved = value; DateArchieved = DateTime.Now; }
        }
        public string FullName { get; set; }

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

        [NotMapped]
        public string Identifier => FullName ?? UserName;
    }
}
