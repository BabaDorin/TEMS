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
    public class EquipmentType: IArchiveableItem
    {
        [Key]
        public string Id { get; set; }
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


        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<EquipmentDefinition> EquipmentDefinitions { get; set; } = new List<EquipmentDefinition>();
        public ICollection<EquipmentType> Children { get; set; } = new List<EquipmentType>();
        public ICollection<EquipmentType> Parents { get; set; } = new List<EquipmentType>();
        public ICollection<ReportTemplate> ReportTemplatesMemberOf { get; set; } = new List<ReportTemplate>();

        [NotMapped]
        public string Identifier => Name;
    }
}
