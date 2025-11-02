using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers.ReusableSnippets
{
    public static class CollectionManipulations
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.Count() == 0;
        }
    }
}
