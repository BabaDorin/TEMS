using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.LibraryEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Managers;

namespace temsAPI.Data.Entities.UserEntities
{
    public class TEMSUser : IdentityUser, IArchiveable, IIdentifiable
    {
        public DateTime DateRegistered { get; set; }

        public DateTime? DateArchieved { get; set; }
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

        [MaxLength(50)]
        public string FullName { get; set; }

        public bool GetEmailNotifications { get; set; }

#nullable enable
        public string? ProfilePhotoFileName { get; set; }
        public string? ProfilePhotoMinifiedFileName { get; set; }

        [ForeignKey("PersonnelId")]
        [InverseProperty("TEMSUser")]
        public Personnel? Personnel { get; set; }

        [MaxLength(150)]
        public string? PersonnelId { get; set; }

        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

        public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
        public virtual ICollection<UserCommonNotification> UserCommonNotifications { get; set; } = new List<UserCommonNotification>();
        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public virtual ICollection<Ticket> ClosedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Ticket> ClosedAndThenReopenedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Equipment> RegisteredEquipment { get; set; } = new List<Equipment>();
        public virtual ICollection<Report.Report> GeneratedReports { get; set; } = new List<Report.Report>();
        public virtual ICollection<Ticket> ArchievedTickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Equipment> ArchievedEquipment { get; set; } = new List<Equipment>();
        public virtual ICollection<Log> CreatedLogs { get; set; } = new List<Log>();
        public virtual ICollection<Log> ArchivedLogs { get; set; } = new List<Log>();
        public virtual ICollection<EquipmentAllocation> ArchivedAllocations { get; set; } = new List<EquipmentAllocation>();
        public virtual ICollection<EquipmentDefinition> ArchivedDefinitions { get; set; } = new List<EquipmentDefinition>();
        public virtual ICollection<EquipmentSpecifications> ArchivedSpecifications { get; set; } = new List<EquipmentSpecifications>();
        public virtual ICollection<EquipmentType> ArchivedTypes { get; set; } = new List<EquipmentType>();
        public virtual ICollection<Property> ArchivedProperties { get; set; } = new List<Property>();
        public virtual ICollection<Key> ArchivedKeys { get; set; } = new List<Key>();
        public virtual ICollection<KeyAllocation> ArchivedKeyAllocations { get; set; } = new List<KeyAllocation>();
        public virtual ICollection<LibraryItem> UploadedLibraryItems { get; set; } = new List<LibraryItem>();
        public virtual ICollection<Personnel> ArchivedPersonnel { get; set; } = new List<Personnel>();
        public virtual ICollection<PersonnelPosition> ArchivedPersonnelPositions { get; set; } = new List<PersonnelPosition>();
        public virtual ICollection<Room> ArchivedRooms { get; set; } = new List<Room>();
        public virtual ICollection<RoomLabel> ArchivedRoomLabels { get; set; } = new List<RoomLabel>();
        public virtual ICollection<ReportTemplate> CreatedReportTemplates { get; set; } = new List<ReportTemplate>();
        public virtual ICollection<ReportTemplate> ArchivedReportTemplates { get; set; } = new List<ReportTemplate>();
        public virtual ICollection<Status> ArchivedStatuses { get; set; } = new List<Status>();

        /// <summary>
        /// Simultates the OnDeleteBehaviour: SetNull, which does not seem to be supported by default by entityframeworkcore.
        /// </summary>
        /// <returns></returns>
        public void ChildrenSetNull()
        {
            CreatedTickets.ForEach(q => q.CreatedById = null);
            ClosedTickets.ForEach(q => q.ClosedById = null);
            RegisteredEquipment.ForEach(q => q.RegisteredByID = null);
            GeneratedReports.ForEach(q => q.GeneratedByID = null);
            ArchievedTickets.ForEach(q => q.ArchievedById = null);
            ArchievedEquipment.ForEach(q => q.ArchievedById = null);
            CreatedLogs.ForEach(q => q.CreatedByID = null);
            ArchivedLogs.ForEach(q => q.ArchievedById = null);
            ArchivedAllocations.ForEach(q => q.ArchievedById = null);
            ArchivedDefinitions.ForEach(q => q.ArchievedById = null);
            ArchivedSpecifications.ForEach(q => q.ArchievedById = null);
            ArchivedTypes.ForEach(q => q.ArchievedById = null);
            ArchivedProperties.ForEach(q => q.ArchievedById = null);
            ArchivedKeys.ForEach(q => q.ArchievedById = null);
            ArchivedKeyAllocations.ForEach(q => q.ArchievedById = null);
            UploadedLibraryItems.ForEach(q => q.UploadedById = null);
            ArchivedPersonnel.ForEach(q => q.ArchievedById = null);
            ArchivedPersonnelPositions.ForEach(q => q.ArchievedById = null);
            ArchivedRooms.ForEach(q => q.ArchievedById = null);
            ArchivedRoomLabels.ForEach(q => q.ArchievedById = null);
            CreatedReportTemplates.ForEach(q => q.CreatedById = null);
            ArchivedReportTemplates.ForEach(q => q.ArchievedById = null);
            ArchivedStatuses.ForEach(q => q.ArchievedById = null);
            Announcements.ForEach(q => q.AuthorID = null);
        }

        [NotMapped]
        public string Identifier => FullName ?? UserName;
    }
}
