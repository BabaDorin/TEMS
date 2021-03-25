using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.Report
{
    public class ReportTemplate
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string SepparateBy { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }
        
        public List<Personnel> Signatories { get; set; }
        public List<Property> Properties { get; set; }
        public List<EquipmentType> EquipmentTypes { get; set; }
        public List<EquipmentDefinition> EquipmentDefinitions { get; set; }
        public List<Personnel> Personnel { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
