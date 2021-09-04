using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Helpers.Filters.Contracts;

namespace temsAPI.Helpers.Filters
{
    public class LogFilter : IPaginationFilter, IEquipmentLabel
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = int.MaxValue;

        public string EquipmentId { get; set; }
        public string RoomId { get; set; }
        public string PersonnelId { get; set; }
        public string SearchValue { get; set; } // for the search bar (if any) (support might be added later)
        public List<string> IncludeLabels { get; set; }
    }
}
