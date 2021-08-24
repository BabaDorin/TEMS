using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.Filters.Contracts;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.Helpers.Filters
{
    public class EquipmentFilter : IPaginationFilter
    {
        private int skip = 0;
        public int Skip
        {
            get
            {
                return skip;
            }
            set
            {
                if (value < 0)
                {
                    skip = 0;
                    return;
                }

                skip = value;
            }
        }

        private int take = int.MaxValue;
        public int Take { get 
            {
                return take;
            }
            set
            {
                if(value <= 0)
                {
                    take = int.MaxValue;
                    return;
                }

                take = value;
            }
        }

        //// Flag that indicates whatever to include equipment of children definitions or not.
        //// It doesn't check for the presence of any parent, but it goes deeper into checking whatever
        //// the equipment's type has any parent
        //// True => Fetch only equipment with definitions that do not have any parent
        //public bool OnlyParents { get; set; }

        //// Flag that indicates the fetcher to include only detached equipment
        //// It only checks agains the presence of any parent for a specific equipment and does not care about
        //// it's definition.
        //// True => Fetch only equipment entities that do not have a parent associated with the equipment itself
        //public bool OnlyDetached { get; set; }

        // Allocatee IDs
        // Empty => Include all Rooms & all Personnel
        public List<string> Rooms { get; set; }
        public List<string> Personnel { get; set; }

        // Other flags
        public bool IncludeParents { get; set; } = true;
        public bool IncludeChildren { get; set; } = true;
        public bool IncludeInUse { get; set; } = true;
        public bool IncludeUnused { get; set; } = true;
        public bool IncludeFunctional { get; set; } = true;
        public bool IncludeDefect { get; set; } = true;
        public bool IncludeAttached { get; set; } = true;
        public bool IncludeDetached { get; set; } = true;

        // Additional include queries that will be merged with default ones
        public Expression<Func<IQueryable<Equipment>, IIncludableQueryable<Equipment, object>>> IncludeFromEquipment { get; set; }
        public Expression<Func<IQueryable<EquipmentAllocation>, IIncludableQueryable<EquipmentAllocation, object>>> IncludeFromEquipmentAllocation { get; set; }

        // Type & Definition filtering (Ids are provided here).
        // Empty or the presence of "any" members => Include all types & all definitions
        public List<string> Types { get; set; }
        public List<string> Definitions { get; set; }

        public bool Validate()
        {
            if (Take == 0)
            {
                Skip = 0;
                Take = int.MaxValue;
            }

            if (!IncludeInUse && !IncludeUnused
                || !IncludeFunctional && !IncludeDefect
                || !IncludeParents && !IncludeChildren
                || !IncludeAttached && !IncludeDetached)
                return false;

            return true;
        }
        
        public bool IsAnyAllocateeSpecified()
        {
            return !Rooms.IsNullOrEmpty() || !Personnel.IsNullOrEmpty();
        }
    }
}
