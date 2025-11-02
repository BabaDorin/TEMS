using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Helpers.ReusableSnippets;
using temsAPI.Validation;

namespace temsAPI.ViewModels.EquipmentDefinition
{
    public class AddEquipmentDefinitionViewModel
    {
        public string Id { get; set; }
        public string TypeId { get; set; }
        public string Identifier { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public List<Option> Properties { get; set; } = new List<Option>();
        public List<AddEquipmentDefinitionViewModel> Children { get; set; } = new List<AddEquipmentDefinitionViewModel>();

        public AddEquipmentDefinitionViewModel()
        {
            Properties = new List<Option>();
            Children = new List<AddEquipmentDefinitionViewModel>();
        }

        /// <summary>
        /// Validates an instance of DefinitionViewModel. If everythink is ok, it returns null, otherwise - 
        /// the error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static async Task<string> Validate(IUnitOfWork unitOfWork, AddEquipmentDefinitionViewModel viewModel, bool isChild = false)
        {
            // If the definition which is being validated is child for another
            if (isChild)
            {
                if(viewModel.Id != null)
                {
                    if (await unitOfWork.EquipmentDefinitions.isExists(q => q.Id == viewModel.Id))
                        return null; 
                 
                    return "One or more child definitions are invalid. Please, review your input.";
                }
            }

            // If it's the update case, we make sure the specified id exists
            // And also, the equipment type should match
            if (viewModel.Id != null)
            {
                var definition = (await unitOfWork.EquipmentDefinitions
                    .Find<Data.Entities.EquipmentEntities.EquipmentDefinition>(q => q.Id == viewModel.Id))
                    .FirstOrDefault();

                if (definition == null)
                    return "Invalid id provided";

                if (definition.EquipmentTypeID != viewModel.TypeId)
                    return "You can't just modify the definition type";
            }

            // Identifier is required
            if (String.IsNullOrEmpty((viewModel.Identifier = viewModel.Identifier.Trim())))
                return "Please provide a valid identifier";

            // Definition with this identifier already exists and it's not the update case
            if (viewModel.Id == null)
                if (await unitOfWork.EquipmentDefinitions
                    .isExists(q => q.Identifier == viewModel.Identifier && !q.IsArchieved))
                    return "There is already a definition having this identifier";

            // Invalid TypeId
            if (!await unitOfWork.EquipmentTypes.isExists(q => q.Id == viewModel.TypeId))
                return "The Equipment Type specified does not exist.";

            // Invalid data for price or currency
            double price;
            if (!double.TryParse(viewModel.Price.ToString(), out price) ||
                price < 0 ||
                (new List<string>() { "lei", "eur", "usd" }).IndexOf(viewModel.Currency) == -1)
                return "Invalid data provided for price or currency";

            // Validating properties
            foreach (var property in viewModel.Properties)
            {
                property.Label = property.Label.Trim();

                if (!await DataTypeValidation.IsValidAsync(property, unitOfWork))
                    return "One or more properties are invalid. Please review your data";
            }

            // Check if children are valid and if the 2 level hierarchy is not violated
            // (There aren't children that also have children)
            StringBuilder stringBuilder = new StringBuilder("");
            foreach(var child in viewModel.Children)
            {
                string validationResult = await Validate(unitOfWork, child, true);
                
                if (validationResult != null)
                {
                    stringBuilder.Append(validationResult + Environment.NewLine);
                    continue;
                }

                var childDefintion = (await unitOfWork.EquipmentDefinitions
                    .Find<Data.Entities.EquipmentEntities.EquipmentDefinition>(
                        where: q => q.Id == child.Id,
                        include: q => q.Include(q => q.Children)))
                    .FirstOrDefault();

                if (childDefintion != null && !childDefintion.Children.IsNullOrEmpty())
                {
                    stringBuilder.Append($"2 level hierarchy violated: {child.Identifier} can not be a child " +
                        $"because it a parent for other definitions" + Environment.NewLine);
                }
            }

            return (stringBuilder.ToString() == "") ? null : stringBuilder.ToString();  
        }

        /// <summary>
        /// Converts an instance of EquipmentDefinition to an instance of AddDefinitionViewModel.
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public static AddEquipmentDefinitionViewModel FromModel(Data.Entities.EquipmentEntities.EquipmentDefinition definition)
        {
            var viewModel = new AddEquipmentDefinitionViewModel
            {
                Id = definition.Id,
                Currency = definition.Currency,
                Description = definition.Description,
                Identifier = definition.Identifier,
                Price = definition.Price,
                TypeId = definition.EquipmentTypeID,
                Properties = definition.EquipmentSpecifications
                .Select(q => new Option
                {
                    Label = q.Property.Name,
                    Value = q.Value,
                }).ToList()
            };

            foreach (var item in definition.Children)
                viewModel.Children.Add(AddEquipmentDefinitionViewModel.FromModel(item));
        
            return viewModel;
        }
    }
}
