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

        public static async Task<EquipmentDefinitionViewModel> FromModel(IUnitOfWork unitOfWork, string modelId)
        {
            var viewModel = (await unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinitionViewModel>(
                        where: q => q.Id == modelId,
                        include: q => q
                        .Include(q => q.Children.Where(q => !q.IsArchieved))
                        .Include(q => q.EquipmentSpecifications)
                        .ThenInclude(q => q.Property).ThenInclude(q => q.DataType)
                        .Include(q => q.Parent)
                        .Include(q => q.EquipmentType),
                        select: q => new EquipmentDefinitionViewModel
                        {
                            Id = q.Id,
                            Identifier = q.Identifier,
                            Currency = q.Currency,
                            Price = q.Price,
                            EquipmentType = new Option
                            {
                                Value = q.EquipmentType.Id,
                                Label = q.EquipmentType.Name
                            },
                            Properties = q.EquipmentSpecifications
                            .Select(q => new ViewPropertyViewModel
                            {
                                Id = q.Property.Id,
                                DisplayName = q.Property.DisplayName,
                                Name = q.Property.Name,
                                Value = q.Value,
                            })
                            .ToList(),
                            Children = q.Children.Select(child=> FromModel(unitOfWork, child.Id).Result).ToList(),
                        }))
                        .FirstOrDefault();

            return viewModel;
        }
    }
}
