using System.Collections.Generic;
using System.Linq;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Helpers
{

    public partial class ReportHelper
    {
        public class RoomSeparator : IEquipmentSeparator
        {
            public IEnumerable<IGrouping<IIdentifiable, Equipment>> GroupEquipment(List<Equipment> equipment)
            {
                return equipment.GroupBy(q => q.EquipmentAllocations
                        .FirstOrDefault(q1 => q1.DateReturned == null) == null
                            ? null
                            : q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null).Room ?? null);
            }
        }
    }
}
