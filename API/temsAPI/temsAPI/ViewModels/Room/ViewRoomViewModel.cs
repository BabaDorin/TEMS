using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.Personnel;

namespace temsAPI.ViewModels.Room
{
    public class ViewRoomViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public int Floor { get; set; }
        public string Description { get; set; }
        public List<ViewPersonnelSimplifiedViewModel> Supervisory { get; set; } 
            = new List<ViewPersonnelSimplifiedViewModel>();
        public int ActiveTickets { get; set; }
        public List<Option> Labels { get; set; } = new List<Option>();
    }
}
