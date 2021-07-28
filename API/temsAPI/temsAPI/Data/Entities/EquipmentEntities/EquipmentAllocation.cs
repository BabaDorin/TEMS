using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class EquipmentAllocation: IArchiveableItem
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment Equipment { get; set; }

        [MaxLength(150)]
        public string EquipmentID { get; set; }

        public DateTime DateAllocated { get; set; }
        
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


#nullable enable
        [InverseProperty("EquipmentAllocations")]
        [ForeignKey("PersonnelID")]
        public Personnel? Personnel { get; set; }

        [MaxLength(150)]
        public string? PersonnelID { get; set; }

        [InverseProperty("EquipmentAllocations")]
        [ForeignKey("RoomID")]
        public Room? Room { get; set; }

        [MaxLength(150)]
        public string? RoomID { get; set; }

        [NotMapped]
        public string Assignee
        {
            get 
            {
                if (Personnel != null)
                    return Personnel.Name;

                if (Room != null)
                    return Room.Identifier;

                return "Undefined";
            }
        }

        [NotMapped]
        public string Identifier => $"Identifier: {Equipment?.TemsIdOrSerialNumber} (${Equipment?.Identifier}): ${Assignee ?? "No assignee found"}";

        public DateTime? DateReturned { get; set; }
        
        [InverseProperty("ArchivedAllocations")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable
    }
}
