using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Helpers.ReportHelpers
{
    public interface ICommonPropertyValueProvider
    {
        public object GetValue(Equipment equipment, PropertyInfo[] properties = null);
    }
}
