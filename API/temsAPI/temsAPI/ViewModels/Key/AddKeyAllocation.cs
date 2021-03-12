using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Key
{
    public class AddKeyAllocation
    {
        public List<string> KeyIds { get; set; } = new List<string>();
        public string PersonnelId { get; set; }
    }
}
