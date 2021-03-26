using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.Property;

namespace temsAPI.ViewModels.Equipment
{
    public class ViewEquipmentViewModel
    {
        public string Id { get; set; }
        public Option Definition { get; set; }
        public string TemsId { get; set; }
        public string SerialNumber { get; set; }
        public IOption Room { get; set; }
        public IOption Personnnel { get; set; }
        public string Type { get; set; }
        public List<ViewPropertyViewModel> SpecificTypeProperties { get; set; }
        public List<Option> Children { get; set; }
        public IOption Parent { get; set; }
        public bool IsUsed { get; set; }
        public bool IsDefect { get; set; }
        public List<string> Photos { get; set; }

        public ViewEquipmentViewModel()
        {
            SpecificTypeProperties = new List<ViewPropertyViewModel>();
            Children = new List<Option>();
            Photos = new List<string>();
        }
    }
}
