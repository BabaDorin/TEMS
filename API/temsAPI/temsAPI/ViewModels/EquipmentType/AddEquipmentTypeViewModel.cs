using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.EquipmentType
{
    public class AddEquipmentTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Option> Parents { get; set; } = new List<Option>();
        public virtual ICollection<Option> Properties{ get; set; } = new List<Option>();
    }
}
