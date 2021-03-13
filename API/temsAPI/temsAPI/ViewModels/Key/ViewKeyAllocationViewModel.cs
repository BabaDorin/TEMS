using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Key
{
    public class ViewKeyAllocationViewModel
    {
        public string Id { get; set; }
        public Option Personnel { get; set; }
        public Option Key { get; set; }
        public Option Room { get; set; }
        public Option AllocatedBy { get; set; }
        public DateTime DateAllocated { get; set; }
        public DateTime? DateReturned { get; set; }
    }
}
