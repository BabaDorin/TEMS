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
        [Key] [MaxLength(150)]
        public string Id { get; set; }
        
        [MaxLength(250)]
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

#nullable enable
        [InverseProperty("CreatedLogs")]
        [ForeignKey("CreatedByID")]
        public TEMSUser? CreatedBy { get; set; }
        public string? CreatedByID { get; set; }

        [ForeignKey("EquipmentID")]
        public Equipment? Equipment { get; set; }

        [MaxLength(150)]
        public string? EquipmentID { get; set; }

        [InverseProperty("Logs")]
        [ForeignKey("RoomID")]
        public Room? Room { get; set; }

        [MaxLength(150)]
        public string? RoomID { get; set; }

        [InverseProperty("Logs")]
        [ForeignKey("PersonnelID")]
        public Personnel? Personnel { get; set; }

        [MaxLength(150)]
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

        /// <summary>
        /// If the subject of the log is an equipment, the this property will store equipment's label at the moment
        /// of generating the log. (Helps for filtering & joinig table is too expesinve for this scope. Moreover, 
        /// we're interested in the label of the equipment at the momemnt of log generation, not when fetching.
        /// </summary>
        [MaxLength(100)]
        public string EquipmentLabel { get; set; }

        [NotMapped]
        public string Identifier => $"Description: {Description}, Equipment: {Equipment?.TemsIdOrSerialNumber ?? "none"}, Personnel: {Personnel?.Name ?? "none"}, Room: {Room?.Identifier ?? "none"}";
    }
}
