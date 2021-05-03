using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class PersonnelRoomSupervisory: IArchiveable, IIdentifiable
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("PersonnelID")]
        public Personnel Personnel { get; set; }
        public string PersonnelID { get; set; }

        [ForeignKey("RoomID")]
        public Room Room { get; set; }
        public string RoomID { get; set; }
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
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

        public string Identifier => $"{Personnel.Name} - {Room.Identifier}";
    }
}
