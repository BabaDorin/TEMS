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
            };

            // BEFREE
            // Switch from IncludeParent / IncludeChildren flags to equipment labels
            if (template.IncludeChildren)
            {
                filter.IncludeLabels.Add("Part");
                filter.IncludeLabels.Add("Component");
            }

            if (template.IncludeParent)
            {
                filter.IncludeLabels.Add("Equipment");
            }

            if (!template.Properties.IsNullOrEmpty())
            {
                filter.IncludeFromEquipment = q => q.Include(q => q.EquipmentDefinition.EquipmentSpecifications);
            }

            return filter;
        }
    }
}
