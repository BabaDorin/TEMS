using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers.Filters.Contracts
{
    public interface IEquipmentLabel
    {
        List<string> IncludeLabels { get; set; }
    }
}
