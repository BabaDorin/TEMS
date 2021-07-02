using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class Ticket: IArchiveableItem, IPinable
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        public bool IsPinned { get; set; }
        
        public int TrackingNumber { get; set; }
        
        public DateTime DateCreated { get; set; }

#nullable enable
        public DateTime? DateClosed { get; set; }

        [ForeignKey("StatusId")]
        public Status? Status { get; set; }

        [MaxLength(150)]
        public string? StatusId { get; set; }

        [ForeignKey("LabelId")]
        public Label? Label { get; set; }

        [MaxLength(150)]
        public string? LabelId { get; set; }

        [InverseProperty("ClosedTickets")]
        [ForeignKey("ClosedById")]
        public TEMSUser? ClosedBy { get; set; }
        public string? ClosedById { get; set; }

        [MaxLength(100)]
        public string? Problem { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }

        [InverseProperty("CreatedTickets")]
        [ForeignKey("CreatedById")]
        public TEMSUser? CreatedBy { get; set; }
        public string? CreatedById { get; set; }

        [InverseProperty("ArchievedTickets")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }

        public DateTime? DatePinned { get; set; }
#nullable disable

        private bool isArchieved;
        public bool IsArchieved
        {
            get
            {
                return isArchieved;
            }
            set
            {
                isArchieved = value;
                DateArchieved = (value)
                    ? DateTime.Now
                    : null;
            }
        }
        public DateTime? DateArchieved { get; set; }

        public ICollection<Personnel> Personnel { get; set; } = new List<Personnel>();
        public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        
        [InverseProperty("AssignedTickets")]
        public ICollection<TEMSUser> Assignees  { get; set; } = new List<TEMSUser>();
        
        // Used primarily for getting the Probability of reopening analytics
        [InverseProperty("ClosedAndThenReopenedTickets")]
        public ICollection<TEMSUser> PreviouslyClosedBy { get; set; } = new List<TEMSUser>(); 

        [NotMapped]
        public string Identifier => $"Problem: {Problem}, Description: {Description}, CreatedBy: {CreatedBy?.FullName ?? CreatedBy?.UserName}";
    }
}
