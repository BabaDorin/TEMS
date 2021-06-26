﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class Log: IArchiveableItem
    {
        [Key]
        public string Id { get; set; }
        
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

#nullable enable

        [InverseProperty("CreatedLogs")]
        [ForeignKey("CreatedByID")]
        public TEMSUser? CreatedBy { get; set; }
        public string? CreatedByID { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment? Equipment { get; set; }
        public string? EquipmentID { get; set; }

        [InverseProperty("Logs")]
        [ForeignKey("RoomID")]
        public Room? Room { get; set; }
        public string? RoomID { get; set; }

        [InverseProperty("Logs")]
        [ForeignKey("PersonnelID")]
        public Personnel? Personnel { get; set; }
        public string? PersonnelID { get; set; }

        [InverseProperty("ArchivedLogs")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

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
        public DateTime? DateArchieved { get; set; }

        [NotMapped]
        public string Identifier => $"Description: {Description}, Equipment: {Equipment?.TemsIdOrSerialNumber ?? "none"}, Personnel: {Personnel?.Name ?? "none"}, Room: {Room?.Identifier ?? "none"}";

    }
}
