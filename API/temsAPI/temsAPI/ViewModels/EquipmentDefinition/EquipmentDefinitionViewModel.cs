using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.EquipmentType;
using temsAPI.ViewModels.Property;

namespace temsAPI.ViewModels.EquipmentDefinition
{
    public class EquipmentDefinitionViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public EquipmentTypeViewModel EquipmentType { get; set; }
        public virtual ICollection<PropertyViewModel> Properties { get; set; }
        public virtual ICollection<EquipmentDefinitionViewModel> Children { get; set; }
        public EquipmentDefinitionViewModel Parent { get; set; }

        public EquipmentDefinitionViewModel()
        {
            EquipmentType = new EquipmentTypeViewModel();
            Properties = new List<PropertyViewModel>();
            Children = new List<EquipmentDefinitionViewModel>();
        }
    }
}
