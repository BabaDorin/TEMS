using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Room
{
    public class ViewRoomSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public int ActiveTickets { get; set; }
        public int AllocatedEquipments { get; set; }
        public bool IsArchieved { get; set; }
    }
}
