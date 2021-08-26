using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.Report;
using temsAPI.Helpers.Filters;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.Helpers.ReportHelpers
{
    public class TemplateEquipmentFilterBuilder
    {
        public EquipmentFilter GetFilter(ReportTemplate template)
        {
            var filter = new EquipmentFilter()
            {
                Types = template.EquipmentTypes?.Select(q => q.Id).ToList(),
                Definitions = template.EquipmentDefinitions?.Select(q => q.Id).ToList(),
                Rooms = template.Rooms?.Select(q => q.Id).ToList(),
                Personnel = template.Personnel?.Select(q => q.Id).ToList(),
                IncludeInUse = template.IncludeInUse,
                IncludeUnused = template.IncludeUnused,
                IncludeFunctional = template.IncludeFunctional,
                IncludeDefect = template.IncludeDefect,
                IncludeParents = template.IncludeParent,
                IncludeChildren = template.IncludeChildren,
            };

            if (!template.Properties.IsNullOrEmpty())
            {
                filter.IncludeFromEquipment = q => q.Include(q => q.EquipmentDefinition.EquipmentSpecifications);
            }

            return filter;
        }
    }
}
