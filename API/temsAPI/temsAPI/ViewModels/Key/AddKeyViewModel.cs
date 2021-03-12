using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Key
{
    public class AddKeyViewModel
    {
        public string Identifier { get; set; }
        public int NumberOfCopies { get; set; }
        public string RoomId { get; set; }
        public string Description { get; set; }
    }
}
