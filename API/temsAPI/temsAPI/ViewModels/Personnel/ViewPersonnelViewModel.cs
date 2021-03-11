using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Personnel
{
    public class ViewPersonnelViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<Option> Positions { get; set; } = new List<Option>();
        public List<Option> RoomSupervisories { get; set; }
        public int ActiveTickets { get; set; }
        public int AllocatedEquipments { get; set; }
    }
}
