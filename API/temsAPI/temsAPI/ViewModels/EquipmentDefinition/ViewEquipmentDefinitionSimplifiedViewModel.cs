using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.EquipmentDefinition
{
    public class ViewEquipmentDefinitionSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string EquipmentType { get; set; }
        public string Parent { get; set; }
        public string Children { get; set; }
    }
}
