using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Equipment
{
    public class ViewEquipmentSimplifiedViewModel
    {
        public string Id { get; set; }
        public string TemsId { get; set; }
        public string SerialNumber { get; set; }
        public string TemsIdOrSerialNumber { get; set; }
        public string Definition { get; set; }
        public string Assignee { get; set; }
        public string Type { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDefect { get; set; }
    }
}
