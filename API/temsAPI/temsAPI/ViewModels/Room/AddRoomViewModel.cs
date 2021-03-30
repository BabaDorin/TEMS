using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Room
{
    public class AddRoomViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public int Floor { get; set; }
        public string Description { get; set; }
        public List<Option> Labels { get; set; } = new List<Option>();
    }
}
