using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class Log: IArchiveableItem
    {
        [Key]
        public string Id { get; set; }

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
        
        public bool IsArchieved { get; set; }
        public DateTime DateArchieved { get; set; }

        [NotMapped]
        public string Identifier => $"LogType: {LogType?.Type}, Text: {Text}, Equipment: {Equipment?.TemsIdOrSerialNumber ?? "none"}, Personnel: {Personnel?.Name ?? "none"}, Room: {Room?.Identifier ?? "none"}";
    }
}
