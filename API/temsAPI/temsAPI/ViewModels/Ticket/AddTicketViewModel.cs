using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Ticket
{
    public class AddTicketViewModel
    {
        public string Problem { get; set; }
        public string ProblemDescription { get; set; }
        public string Status { get; set; }
        public List<Option> Rooms { get; set; }
        public List<Option> Personnel { get; set; }
        public List<Option> Equipments { get; set; }
        public List<Option> Assignees { get; set; }
        // Created by
    }
}
