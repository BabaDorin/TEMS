using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.EquipmentType
{
    public class ViewEquipmentTypeSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Editable { get; set; }
        public string Properties { get; set; }
        public List<string> Parents { get; set; }
        public List<string> Children { get; set; }

        public static ViewEquipmentTypeSimplifiedViewModel FromModel(Data.Entities.EquipmentEntities.EquipmentType model)
        {
            return new ViewEquipmentTypeSimplifiedViewModel()
            {
                Id = model.Id,
                Name = model.Name,
                Editable = (bool)model.EditableTypeInfo,
                Children = model.Children
                        .Where(q => !q.IsArchieved)
                        .Select(q => q.Name)
                        .ToList(),
                Parents = model.Parents
                        .Where(q => !q.IsArchieved)
                        .Select(q => q.Name)
                        .ToList(),
            };
        }
    }
}
