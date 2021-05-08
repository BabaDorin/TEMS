using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.ViewModels.EquipmentType;
using temsAPI.ViewModels.Property;

namespace temsAPI.ViewModels.EquipmentDefinition
{
    public class EquipmentDefinitionViewModel
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public Option EquipmentType { get; set; }
        public virtual ICollection<ViewPropertyViewModel> Properties { get; set; }
        public virtual ICollection<EquipmentDefinitionViewModel> Children { get; set; }
        public EquipmentDefinitionViewModel Parent { get; set; }

        public EquipmentDefinitionViewModel()
        {
            EquipmentType = new Option();
            Properties = new List<ViewPropertyViewModel>();
            Children = new List<EquipmentDefinitionViewModel>();
        }

        public static EquipmentDefinitionViewModel FromModel(Data.Entities.EquipmentEntities.EquipmentDefinition model)
        {
            var viewModel = new EquipmentDefinitionViewModel
            {
                Id = model.Id,
                Identifier = model.Identifier,
                Currency = model.Currency,
                Price = model.Price,
                EquipmentType = (model.EquipmentType == null)
                    ? null
                    : new Option
                    {
                        Value = model.EquipmentType.Id,
                        Label = model.EquipmentType.Name
                    },
                Properties = model.EquipmentSpecifications?
                .Select(q => new ViewPropertyViewModel
                {
                    Id = q.Property.Id,
                    DisplayName = q.Property.DisplayName,
                    Name = q.Property.Name,
                    Value = q.Value,
                })
                .ToList(),
                Children = model.Children?.Select(child => FromModel(child)).ToList()
            };

            return viewModel;
        }
    }
}
