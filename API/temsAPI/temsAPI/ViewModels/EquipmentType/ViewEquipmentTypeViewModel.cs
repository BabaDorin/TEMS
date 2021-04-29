using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.Property;

namespace temsAPI.ViewModels.EquipmentType
{
    public class ViewEquipmentTypeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Editable { get; set; }
        public virtual ICollection<ViewPropertyViewModel> Properties { get; set; }
        public virtual ICollection<Option> Parents { get; set; }
        public virtual ICollection<ViewEquipmentTypeViewModel> Children { get; set; }

        public static ViewEquipmentTypeViewModel ParseEquipmentType(
            Data.Entities.EquipmentEntities.EquipmentType equipmentType)
        {
            return new ViewEquipmentTypeViewModel
            {
                Id = equipmentType.Id,
                Name = equipmentType.Name,
                Editable = (bool)equipmentType.EditableTypeInfo,
                Properties = equipmentType.Properties
                            .Where(q => !q.IsArchieved)
                            .Select(q => new ViewPropertyViewModel
                            {
                                Description = q.Description,
                                DataType = q.DataType.Name,
                                DisplayName = q.DisplayName,
                                Id = q.Id,
                                Max = q.Max == null ? 0 : (int)q.Max,
                                Min = q.Min == null ? 0 : (int)q.Min,
                                Name = q.Name,
                                Required = q.Required,
                            }).ToList(),
                Parents = equipmentType.Parents.Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToList(),
                Children = equipmentType.Children.Select(q => ParseEquipmentType(q))
                .ToList()
            };
        }
    }
}
