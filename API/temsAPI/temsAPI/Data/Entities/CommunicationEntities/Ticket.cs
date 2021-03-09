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

#nullable enable
        public DateTime? DateClosed { get; set; }

        [ForeignKey("StatusId")]
        public Status? Status { get; set; }
        public string? StatusId { get; set; }

        [ForeignKey("LabelId")]
        public Label? Label { get; set; }
        public string? LabelId { get; set; }

        //[ForeignKey("PersonnelId")]
        //public Personnel? Personnel { get; set; }
        //public string? PersonnelId { get; set; }


        [ForeignKey("ClosedById")]
        public TEMSUser? ClosedBy { get; set; }
        public string? ClosedById { get; set; }

        public string? Problem { get; set; }
        public string? Description { get; set; }
        
        //[ForeignKey("EquipmentId")]
        //public Equipment? Equipment { get; set; }
        //public string? EquipmentId { get; set; }

        //[ForeignKey("RoomID")]
        //public  Room?  Room { get; set; }
        //public string? RoomID { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }

        public ICollection<Personnel> Personnel { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
