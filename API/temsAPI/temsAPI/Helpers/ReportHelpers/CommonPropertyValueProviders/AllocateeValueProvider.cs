﻿using System.Linq;
using System.Reflection;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.ReportHelpers;

namespace temsAPI.Helpers
{
    public partial class ReportHelper
    {
        public class AllocateeValueProvider : ICommonPropertyValueProvider
        {
            public object GetValue(Equipment equipment, PropertyInfo[] properties = null)
            {
                var lastAllocation = equipment.EquipmentAllocations
                    .FirstOrDefault(q => q.DateReturned == null);

                return lastAllocation == null
                    ? null
                    : "Personnel: " + lastAllocation.Personnel?.Name 
                        ?? "Room: " + lastAllocation.Room?.Identifier;
            }
        }
    }
}
