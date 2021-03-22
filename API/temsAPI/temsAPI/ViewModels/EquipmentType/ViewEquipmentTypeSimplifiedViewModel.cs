using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.EquipmentType
{
    public class ViewEquipmentTypeSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Properties { get; set; }
        public string Parent { get; set; }
        public string Children { get; set; }
    }
}
