using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.EquipmentType
{
    public class AddEquipmentTypeViewModel
    {
        public List<Option>? Parents { get; set; }
        public string Name { get; set; }
        public List<Option> Properties{ get; set; }
    }
}
