using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
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
        public virtual ICollection<Option> Properties { get; set; }
        public virtual ICollection<AddEquipmentDefinitionViewModel> Children { get; set; }

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

            StringBuilder stringBuilder = new StringBuilder("");
            foreach(var child in viewModel.Children)
            {
                string validationResult = await Validate(unitOfWork, child, true);
                if (validationResult != null)
                    stringBuilder.Append(validationResult + Environment.NewLine);
            }

            return (stringBuilder.ToString() == "") ? null : stringBuilder.ToString();  
        }
    }
}
