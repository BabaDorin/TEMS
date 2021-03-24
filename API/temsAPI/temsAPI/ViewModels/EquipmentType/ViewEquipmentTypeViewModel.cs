using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.Property;

namespace temsAPI.ViewModels.EquipmentType
{
    public class ViewEquipmentTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<PropertyViewModel> Properties { get; set; }
        public virtual ICollection<Option> Parents { get; set; }
        public virtual ICollection<ViewEquipmentTypeViewModel> Children { get; set; }
    }
}
