using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class Log
    {
        [Key]
        public string ID { get; set; }

        public DateTime DateCreated { get; set; }
#nullable enable
        public string? Text { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment? Equipment { get; set; }
        public string? EquipmentID { get; set; }

        [ForeignKey("RoomID")]
        public Room? Room { get; set; }
        public string? RoomID { get; set; }

        [ForeignKey("PersonnelID")]
        public Personnel? Personnel { get; set; }
        public string? PersonnelID { get; set; }

        public bool IsImportant { get; set; }
        
        [ForeignKey("LogTypeID")]
        public LogType? LogType { get; set; }
        public string? LogTypeID { get; set; }
#nullable disable
    }
}
