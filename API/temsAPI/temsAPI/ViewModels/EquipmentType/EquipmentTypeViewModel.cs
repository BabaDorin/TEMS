using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.Property;

namespace temsAPI.ViewModels.EquipmentType
{
    public class EquipmentTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<PropertyViewModel> Properties { get; set; }
        public List<EquipmentTypeViewModel> Children { get; set; }
    }
}
