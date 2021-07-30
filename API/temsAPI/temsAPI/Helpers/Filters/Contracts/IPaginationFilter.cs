using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers.Filters.Contracts
{
    interface IPaginationFilter
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
