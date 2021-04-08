using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(Identifier))]
    public class EquipmentDefinition: IArchiveableItem
    {
        [Key]
        public string Id { get; set; }

        public string Identifier { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; } = "lei";


#nullable enable
        [ForeignKey("EquipmentTypeID")]
        public EquipmentType? EquipmentType { get; set; }
        public string? EquipmentTypeID { get; set; }

        [ForeignKey("ParentID")]
        public EquipmentDefinition? Parent { get; set; }
        public string? ParentID { get; set; }
        public string? Description { get; set; }

#nullable disable

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

        public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
        public virtual ICollection<EquipmentSpecifications> EquipmentSpecifications { get; set; } = new List<EquipmentSpecifications>();
        public virtual ICollection<EquipmentDefinition> Children { get; set; } = new List<EquipmentDefinition>();
        public virtual ICollection<ReportTemplate> ReportTemplatesMemberOf { get; set; } = new List<ReportTemplate>();
    }
}
