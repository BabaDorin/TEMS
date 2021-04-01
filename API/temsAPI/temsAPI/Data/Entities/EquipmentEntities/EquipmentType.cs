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
        public DateTime DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get { return isArchieved; }
            set { isArchieved = value; DateArchieved = DateTime.Now; }
        }


        public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
        public virtual ICollection<EquipmentDefinition> EquipmentDefinitions { get; set; } = new List<EquipmentDefinition>();
        public virtual ICollection<EquipmentType> Children { get; set; } = new List<EquipmentType>();
        public virtual ICollection<EquipmentType> Parents { get; set; } = new List<EquipmentType>();
        public virtual ICollection<ReportTemplate> ReportTemplatesMemberOf { get; set; } = new List<ReportTemplate>();

        [NotMapped]
        public string Identifier => Name;
    }
}
