using System.Collections.Generic;
using System.Linq;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Helpers
{

    public partial class ReportHelper
    {
        public class NoneSeparator : IEquipmentSeparator
        {
            public string DefaultKeyName { get; set; }

            public IEnumerable<IGrouping<IIdentifiable, Equipment>> GroupEquipment(IEnumerable<Equipment> equipment)
            {
                var groups = new List<Grouping<IIdentifiable, Equipment>>();
                groups.Add(new Grouping<IIdentifiable, Equipment>(null, equipment));
                return groups;
            }
        }
    }
}
