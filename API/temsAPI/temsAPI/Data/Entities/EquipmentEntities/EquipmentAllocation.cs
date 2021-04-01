using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class EquipmentAllocation: IArchiveable, IIdentifiable
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment Equipment { get; set; }
        public string EquipmentID { get; set; }
        public DateTime DateAllocated { get; set; }
        public DateTime DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get { return isArchieved; }
            set { isArchieved = value; DateArchieved = DateTime.Now; }
        }


#nullable enable
        [ForeignKey("PersonnelID")]
        public Personnel? Personnel { get; set; }
        public string? PersonnelID { get; set; }

        [ForeignKey("RoomID")]
        public Room? Room { get; set; }
        public string? RoomID { get; set; }

        [NotMapped]
        public string Assignee
        {
            get => (Room == null) ? Personnel.Name : Room.Identifier;
        }

        [NotMapped]
        public string Identifier => $"{Equipment.TemsIdOrSerialNumber} (${Equipment.Identifier}): ${Assignee}";

        public DateTime? DateReturned { get; set; }

#nullable disable
    }
}
