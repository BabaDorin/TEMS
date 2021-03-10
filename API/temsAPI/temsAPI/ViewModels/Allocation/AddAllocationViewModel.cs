using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Allocation
{
    public class AddAllocationViewModel
    {
        //    equipment: IOption[];
        //allocateToType: string;
        //allocateToId: string;

        public List<Option> Equipments { get; set; } = new List<Option>();
        public string AllocatedToType { get; set; }
        public string AllocatedToId { get; set; }
    }
}
