using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.Report;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class EquipmentType
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsArchieved { get; set; }

        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
        public virtual ICollection<EquipmentDefinition> EquipmentDefinitions { get; set; } = new List<EquipmentDefinition>();
        public virtual ICollection<EquipmentType> Children { get; set; } = new List<EquipmentType>();
        public virtual ICollection<EquipmentType> Parents { get; set; } = new List<EquipmentType>();
        public virtual ICollection<ReportTemplate> ReportTemplatesMemberOf { get; set; } = new List<ReportTemplate>();
    }
}
