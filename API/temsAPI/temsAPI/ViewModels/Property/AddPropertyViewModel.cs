using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.Property
{
    public class AddPropertyViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DataType { get; set; }
        public bool Required { get; set; } = false;
#nullable enable
        public string? Description { get; set; }
#nullable disable
    }
}
