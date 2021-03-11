using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Allocation
{
    public class AddAllocationViewModel
    {
        public List<Option> Equipments { get; set; } = new List<Option>();
        public string AllocateToType { get; set; }
        public string AllocateToId { get; set; }
    }
}
