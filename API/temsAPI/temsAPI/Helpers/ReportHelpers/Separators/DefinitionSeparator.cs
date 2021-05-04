using System.Collections.Generic;
using System.Linq;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Helpers
{

    public partial class ReportHelper
    {
        public class DefinitionSeparator : IEquipmentSeparator
        {
            public IEnumerable<IGrouping<IIdentifiable, Equipment>> GroupEquipment(List<Equipment> equipment)
            {
                return equipment.GroupBy(q => q.EquipmentDefinition).ToList();
            }
        }
    }
}
