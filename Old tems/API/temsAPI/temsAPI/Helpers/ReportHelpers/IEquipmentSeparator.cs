using System.Collections.Generic;
using System.Linq;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.Helpers
{

    public partial class ReportHelper
    {
        public interface IEquipmentSeparator
        {
            /// <summary>
            /// Key name when grouping by null
            /// </summary>
            public string DefaultKeyName { get; set; }

            IEnumerable<IGrouping<IIdentifiable, Equipment>> GroupEquipment(IEnumerable<Equipment> equipment);
        }
    }
}
