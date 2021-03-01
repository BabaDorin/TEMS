using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class PersonnelRoomSupervisory
    {
        [Key]
        public string ID { get; set; }

        [ForeignKey("PersonnelID")]
        public Personnel Personnel { get; set; }
        public string PersonnelID { get; set; }

        [ForeignKey("RoomID")]
        public Room Room { get; set; }
        public string RoomID { get; set; }

        public DateTime DateSet { get; set; }
#nullable enable
        public DateTime? DateCanceled { get; set; }
#nullable disable 
    }
}
