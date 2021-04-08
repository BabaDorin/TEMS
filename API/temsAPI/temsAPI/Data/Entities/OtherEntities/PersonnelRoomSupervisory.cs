using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

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
            get { return isArchieved; }
            set { isArchieved = value; DateArchieved = DateTime.Now; }
        }

        public string Identifier => $"{Personnel.Name} - {Room.Identifier}";
    }
}
