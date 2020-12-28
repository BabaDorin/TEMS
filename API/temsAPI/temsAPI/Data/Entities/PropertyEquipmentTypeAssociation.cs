using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities
{
    public class PropertyEquipmentTypeAssociation
    {
        [ForeignKey("TypeID")]
        public EquipmentType Type { get; set; }
        public string TypeID { get; set; }

        [ForeignKey("PropertyID")]
        public Property Property { get; set; }
        public string PropertyID { get; set; }
    }
}
