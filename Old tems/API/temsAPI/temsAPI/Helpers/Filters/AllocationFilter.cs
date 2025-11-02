using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Helpers.Filters.Contracts;

namespace temsAPI.Helpers.Filters
{
    public class AllocationFilter : IPaginationFilter, IEquipmentLabel
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = int.MaxValue;

        public List<string> Personnel { get; set; }
        public List<string> Rooms { get; set; }
        public List<string> Definitions { get; set; }
        public List<string> Equipment { get; set; } 
        public List<string> IncludeLabels { get; set; } // Equipment, Component, Part (Case sensitive)
        public List<string> IncludeStatuses { get; set; } // Active, Closed (Case sensitive)
    }
}
