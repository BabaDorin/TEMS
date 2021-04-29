using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Property
{
    public class ViewPropertySimplifiedViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Editable { get; set; }
    }
}
