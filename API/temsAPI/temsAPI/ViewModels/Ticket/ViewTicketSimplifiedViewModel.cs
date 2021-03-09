using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Ticket
{
    public class ViewTicketSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Problem { get; set; }
        public string Status { get; set; }
        public Option Label { get; set; }
        public string Description { get; set; }
        public List<Option> Personnel { get; set; }
        public List<Option> Equipments { get; set; }
        public List<Option> Rooms { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateClosed { get; set; }
    }
}
