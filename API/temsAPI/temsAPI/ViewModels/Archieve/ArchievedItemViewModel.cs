using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Archieve
{
    public class ArchievedItemViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public DateTime DateArchieved { get; set; }
        public Option ArchievedBy { get; set; }
    }
}
