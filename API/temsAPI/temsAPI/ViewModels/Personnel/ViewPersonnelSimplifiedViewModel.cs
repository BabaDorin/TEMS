using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Personnel
{
    public class ViewPersonnelSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int AllocatedEquipments { get; set; }
        public int ActiveTickets { get; set; }
        public string Positions { get; set; }
        public bool IsArchieved { get; set; }
    }
}
