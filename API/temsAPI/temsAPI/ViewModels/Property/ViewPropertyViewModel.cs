using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using temsAPI.Data.Entities.EquipmentEntities;

namespace temsAPI.ViewModels.Property
{
    public class ViewPropertyViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public dynamic Value { get; set; }
        public bool Required { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public List<Option> Options { get; set; }
    }
}
