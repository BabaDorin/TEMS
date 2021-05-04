using System.Linq;
using System.Reflection;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.ReportHelpers;

namespace temsAPI.Helpers
{
    public partial class ReportHelper
    {
        public class DefaultCommonPropertyValueProvider : ICommonPropertyValueProvider
        {
            private string propName;
            public DefaultCommonPropertyValueProvider(string propName)
            {
                this.propName = propName;
            }

            public object GetValue(Equipment equipment, PropertyInfo[] properties = null)
            {
                if (properties == null)
                    properties = typeof(Equipment).GetProperties();

                return properties.First(q => q.Name.ToLower() == propName.ToLower())?
                        .GetValue(equipment);
            }
        }
    }
}
