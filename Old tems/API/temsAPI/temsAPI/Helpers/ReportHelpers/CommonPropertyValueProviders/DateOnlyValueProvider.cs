using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Helpers.ReportHelpers.CommonPropertyValueProviders
{
    public class DateOnlyValueProvider : ICommonPropertyValueProvider
    {
        private string propName;

        public DateOnlyValueProvider(string propName)
        {
            this.propName = propName;
        }

        public object GetValue(Equipment equipment, PropertyInfo[] properties = null)
        {
            if (properties == null)
                properties = typeof(Equipment).GetProperties();

            var propertyValue = properties
                .FirstOrDefault(q => q.Name.ToLower() == propName.ToLower())
                ?.GetValue(equipment);
          
            return propertyValue == null ? DBNull.Value : ((DateTime)propertyValue).ToShortDateString();
        }
    }
}
