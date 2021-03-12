using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.ViewModels.Allocation
{
    public class ViewAllocationSimplifiedViewModel
    {
        public string Id { get; set; }
        public Option Equipment { get; set; }
        public Option Personnel { get; set; }
        public Option Room { get; set; }
        public DateTime DateAllocated { get; set; }
        public DateTime? DateReturned { get; set; }
    }
}
