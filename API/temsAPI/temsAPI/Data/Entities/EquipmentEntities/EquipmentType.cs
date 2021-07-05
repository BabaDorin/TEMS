using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class EquipmentType: IArchiveableItem
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

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
        [DefaultValue(true)]
        public bool? EditableTypeInfo { get; set; } = true;

        [InverseProperty("ArchivedTypes")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<EquipmentDefinition> EquipmentDefinitions { get; set; } = new List<EquipmentDefinition>();
        public ICollection<EquipmentType> Children { get; set; } = new List<EquipmentType>();
        public ICollection<EquipmentType> Parents { get; set; } = new List<EquipmentType>();
        public ICollection<ReportTemplate> ReportTemplatesMemberOf { get; set; } = new List<ReportTemplate>();

        [NotMapped]
        public string Identifier => Name;
    }
}
