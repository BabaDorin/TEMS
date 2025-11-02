using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers.Filters.Contracts
{
    interface IPaginationFilter
    {
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
