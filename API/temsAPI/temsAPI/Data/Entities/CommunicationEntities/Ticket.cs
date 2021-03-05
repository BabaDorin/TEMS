using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class Ticket
    {
        [Key]
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateClosed { get; set; }

#nullable enable
        [ForeignKey("AuthorID")]
        public Personnel? Author { get; set; }
        public string? AuthorID { get; set; }
        
        public string? AuthorName { get; set; }


        [ForeignKey("ClosedByID")]
        public TEMSUser? ClosedBy { get; set; }
        public string? ClosedByID { get; set; }

        public string? Problem { get; set; }
        public string? Description { get; set; }
        
        [ForeignKey("EquipmentID")]
        public Equipment? Equipment { get; set; }
        public string? EquipmentID { get; set; }

        [ForeignKey("RoomID")]
        public  Room?  Room { get; set; }
        public string? RoomID { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }
    }
}
