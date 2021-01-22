using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class Ticket
    {
        [Key]
        public string ID { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateClosed { get; set; }

        [ForeignKey("AuthorID")]
        public Personnel Author { get; set; }
#nullable enable
        public string? AuthorID { get; set; }

        public string? AuthorName { get; set; }
#nullable disable


        [ForeignKey("ClosedByID")]
        public IdentityUser ClosedBy { get; set; }
#nullable enable
        public string? ClosedByID { get; set; }

        public string? Problem { get; set; }
        public string? Description { get; set; }
#nullable disable

        [ForeignKey("EquipmentID")]
        public Equipment Equipment { get; set; }
#nullable enable
        public string? EquipmentID { get; set; }
#nullable disable


        [ForeignKey("RoomID")]
        public  Room  Room { get; set; }
#nullable enable
        public string? RoomID { get; set; }
#nullable disable
    }
}
