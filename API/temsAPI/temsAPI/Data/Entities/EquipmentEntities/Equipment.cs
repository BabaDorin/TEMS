using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(TEMSID))]
    [Index(nameof(SerialNumber))]
    public class Equipment
    {
        [Key]
        public string Id { get; set; }

#nullable enable
        [ForeignKey("ParentID")]
        public Equipment? Parent { get; set; }
        public string? ParentID { get; set; }

        public string? TEMSID { get; set; }
        public string? SerialNumber { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }

        [ForeignKey("EquipmentDefinitionID")]
        public EquipmentDefinition? EquipmentDefinition { get; set; }
        public string? EquipmentDefinitionID { get; set; }

        [ForeignKey("RegisteredByID")]
        public TEMSUser? RegisteredBy { get; set; }
        public string? RegisteredByID { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? DeletedDate { get; set; }
#nullable disable

        public bool IsDefect { get; set; }
        public bool IsUsed { get; set; }

        public virtual ICollection<Log> Logs { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Equipment> Children { get; set; }
        public virtual ICollection<RoomEquipmentAllocation> RoomEquipmentAllocations { get; set; }
        public virtual ICollection<PersonnelEquipmentAllocation> PersonnelEquipmentAllocations { get; set; }
    }
}
