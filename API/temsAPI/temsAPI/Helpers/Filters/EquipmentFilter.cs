using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.Filters.Contracts;

namespace temsAPI.Helpers.Filters
{
    public class EquipmentFilter : IPaginationFilter
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }

        public bool OnlyParents { get; set; }
        public List<string> Rooms { get; set; }
        public List<string> Personnel { get; set; }
        public List<string> Types { get; set; }

        public bool Validate()
        {
            if (ItemsPerPage == 0)
            {
                PageNumber = 0;
                ItemsPerPage = int.MaxValue;
            }

            return true;
        }

        public int GetSkip()
        {
            return PageNumber * ItemsPerPage;
        }

        public int GetTake()
        {
            return ItemsPerPage;
        }
    }
}
