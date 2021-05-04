using System.Reflection;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Helpers.ReportHelpers;

namespace temsAPI.Helpers
{
    public partial class ReportHelper
    {
        public class TypeValueProvider : ICommonPropertyValueProvider
        {
            public object GetValue(Equipment equipment, PropertyInfo[] properties = null)
            {
                return equipment.EquipmentDefinition?.EquipmentType?.Name;
            }
        }
    }
}
