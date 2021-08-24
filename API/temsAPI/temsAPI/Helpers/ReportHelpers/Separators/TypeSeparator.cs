using System.Collections.Generic;
using System.Linq;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Helpers
{

    public partial class ReportHelper
    {
        public class TypeSeparator : IEquipmentSeparator
        {
            public string DefaultKeyName { get; set; } = "Other";

            public IEnumerable<IGrouping<IIdentifiable, Equipment>> GroupEquipment(IEnumerable<Equipment> equipment)
            {
                return equipment.GroupBy(q => q.EquipmentDefinition.EquipmentType).ToList();
            }
        }
    }
}
