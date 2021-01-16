using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(TEMSID))]
    [Index(nameof(SerialNumber))]
    public class Equipment
    {
        [Key]
        public string ID { get; set; }

        [ForeignKey("ParentID")]
        public Equipment Parent { get; set; }
        public string? ParentID { get; set; }

        public string? TEMSID { get; set; }
        public string? SerialNumber { get; set; }
        public double? Price { get; set; }
        public double? Commentary { get; set; }

        [ForeignKey("EquipmentDefinitionID")]
        public EquipmentDefinition EquipmentDefinition { get; set; }
        public string EquipmentDefinitionID { get; set; }

        public DateTime? PurchaseDate { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? DeletedDate { get; set; }

        public bool IsDefect { get; set; }
        public bool IsUsed { get; set; }
    }
}
