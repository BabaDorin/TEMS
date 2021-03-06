using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.EquipmentDefinition
{
    public class AddEquipmentDefinitionViewModel
    {
        public string TypeId { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public List<Option> Properties { get; set; }
        public List<AddEquipmentDefinitionViewModel> Children { get; set; }

        public AddEquipmentDefinitionViewModel()
        {
            Properties = new List<Option>();
            Children = new List<AddEquipmentDefinitionViewModel>();
        }
    }
}
