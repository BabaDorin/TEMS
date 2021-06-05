using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.EquipmentDefinition
{
    public class ViewEquipmentDefinitionSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string EquipmentType { get; set; }
        public string Parent { get; set; }
        public List<string> Children { get; set; }

        public static ViewEquipmentDefinitionSimplifiedViewModel FromModel(Data.Entities.EquipmentEntities.EquipmentDefinition model)
        {
            return new ViewEquipmentDefinitionSimplifiedViewModel()
            {
                Id = model.Id,
                Identifier = model.Identifier,
                Parent = model.Parent != null
                                ? model.Parent.Identifier
                                : null,
                Children = model.Children
                                .Where(q => !q.IsArchieved)
                                .Select(q => q.Identifier)
                                .ToList(),
                EquipmentType = model.EquipmentType.Name
            };
        }
    }
}
