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
    public class Room : IArchiveableItem
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(100)]
        public string Identifier { get; set; }

#nullable enable
        public int? Floor { get; set; }

        [MaxLength(1500)]
        public string? Description { get; set; }

        [InverseProperty("ArchivedRooms")]
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
        public virtual ICollection<Personnel> Supervisories { get; set; } = new List<Personnel>();
        public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
        public virtual ICollection<Ticket> Tickets  { get; set; } = new List<Ticket>();
        public virtual ICollection<RoomLabel> Labels { get; set; } = new List<RoomLabel>();
        public virtual ICollection<Key> Keys { get; set; } = new List<Key>();
        public virtual ICollection<ReportTemplate> ReportTemplatesMemberOf { get; set; } = new List<ReportTemplate>();

        public async Task AssignLabels(List<string> labelIds, IUnitOfWork unitOfWork)
        {
            Labels.Clear();
            if (labelIds != null && labelIds.Count > 0)
                Labels = await unitOfWork.RoomLabels.FindAll<RoomLabel>(
                    where: q => labelIds.Contains(q.Id));
        }

        public async Task AssignSupervisories(List<string> supervisoriesIds, IUnitOfWork unitOfWork)
        {
            Supervisories.Clear();
            if (supervisoriesIds != null && supervisoriesIds.Count > 0)
                Supervisories = await unitOfWork.Personnel.FindAll<Personnel>(
                    where: q => supervisoriesIds.Contains(q.Id));
        }
    }
}
