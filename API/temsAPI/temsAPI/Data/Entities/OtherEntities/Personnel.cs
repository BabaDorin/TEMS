using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Personnel: IArchiveableItem
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
#nullable enable
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }

        public string? ImagePath { get; set; }

        public TEMSUser? TEMSUser { get; set; }

        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

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

        public virtual ICollection<EquipmentAllocation> EquipmentAllocations { get; set; } = new List<EquipmentAllocation>();
        public virtual ICollection<Room> RoomsSupervisoried { get; set; } = new List<Room>();
        public virtual ICollection<KeyAllocation> KeyAllocations { get; set; } = new List<KeyAllocation>();
        public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<PersonnelPosition> Positions { get; set; } = new List<PersonnelPosition>();

        [InverseProperty("Signatories")]
        public virtual ICollection<ReportTemplate> ReportTemplatesAssigned { get; set; } = new List<ReportTemplate>();
        [InverseProperty("Personnel")]
        public virtual ICollection<ReportTemplate> ReportTemplatesMember { get; set; } = new List<ReportTemplate>();

        [NotMapped]
        public string Identifier => Name;

        // BEFREE: Add multiple Emails and Phone numbers support later if needed.

        public async Task AssignPositions(List<string> positionIds, IUnitOfWork unitOfWork)
        {
            Positions.Clear();
            Positions = await unitOfWork.PersonnelPositions.FindAll<PersonnelPosition>(
                   where: q => positionIds.Contains(q.Id));
        }

        public async Task AssignUser(string userId, IUnitOfWork unitOfWork)
        {
            TEMSUser = (await unitOfWork.TEMSUsers
                .Find<TEMSUser>(
                    where: q => q.Id == userId
                )).FirstOrDefault();
        }

        public void CancelUserConnection()
        {
            TEMSUser = null;
        }
    }
}
