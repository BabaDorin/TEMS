using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.EquipmentType
{
    public class AddEquipmentTypeViewModel
    {
        public virtual ICollection<Option>? Parents { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Option> Properties{ get; set; }
    }
}
