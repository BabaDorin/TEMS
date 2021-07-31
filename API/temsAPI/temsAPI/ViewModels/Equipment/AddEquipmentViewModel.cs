using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Helpers.ReusableSnippets;

namespace temsAPI.ViewModels.Equipment
{
    public class AddEquipmentViewModel
    {
        public string Id { get; set; }
        public string EquipmentDefinitionID { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public bool IsDefect { get; set; }
        public bool IsUsed { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SerialNumber { get; set; }
        public string Temsid { get; set; }
        public List<AddEquipmentViewModel> Children { get; set; } = new List<AddEquipmentViewModel>();

        /// <summary>
        /// Validates an instance of AddEquipmentViewModel. Returns null if everything is ok, otherwise - returns
        /// an error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static async Task<string> Validate(IUnitOfWork unitOfWork, AddEquipmentViewModel viewModel)
        {
            Data.Entities.EquipmentEntities.Equipment updateModel = null;

            // It's the update case and the provided id is invalid
            if (viewModel.Id != null)
            {
                updateModel = (await unitOfWork.Equipments
                    .Find<Data.Entities.EquipmentEntities.Equipment>(
                        where: q => q.Id == viewModel.Id))
                    .FirstOrDefault();

                if (updateModel == null)
                    return "Invalid id provided";
            }

            // at least one (TEMSID or SerialNumber) should be provided
            viewModel.Temsid = viewModel.Temsid?.Trim();
            viewModel.SerialNumber = viewModel.SerialNumber?.Trim();
            if (String.IsNullOrEmpty(viewModel.Temsid) && String.IsNullOrEmpty(viewModel.SerialNumber))
                return "Please, provide information for TemsID and / or SerialNumber";

            // Equipment already exists and it's not the update case
            if (updateModel == null)
            {
                if (!String.IsNullOrEmpty(viewModel.Temsid) &&
                    await unitOfWork.Equipments.isExists(q => q.TEMSID == viewModel.Temsid) ||
                    !String.IsNullOrEmpty(viewModel.SerialNumber) &&
                    await unitOfWork.Equipments.isExists(q => q.SerialNumber == viewModel.SerialNumber))
                    return "An equipment with the same TEMSID or Serial number already exists.";
            }
            else
            {
                if (viewModel.Temsid != updateModel.TEMSID
                    && 
                    await unitOfWork.Equipments
                    .isExists(q => q.TEMSID == viewModel.Temsid && !q.IsArchieved))
                    return "An equipment with the specified TEMSID already exists";

                if (viewModel.SerialNumber != updateModel.SerialNumber
                    && 
                    await unitOfWork.Equipments
                    .isExists(q => q.SerialNumber == viewModel.SerialNumber && !q.IsArchieved))
                    return "An equipment with the specified Serial number already exists";
            }

            // No value provided for purchase date
            if (viewModel.PurchaseDate == new DateTime())
                viewModel.PurchaseDate = DateTime.Now;

            // Invalid price data
            if (viewModel.Price < 0 ||
                (new List<string> { "lei", "eur", "usd" }).IndexOf(viewModel.Currency) == -1)
                return "Invalid price data provided.";

            // Invalid definition provided
            // Case 1: Invalid id
            if (!await unitOfWork.EquipmentDefinitions.isExists(q => q.Id == viewModel.EquipmentDefinitionID))
                return "An equipment definition having the specified id has not been found.";

            // Case 2: It's the update case and the new definition is different from the old one
            if (updateModel != null && viewModel.EquipmentDefinitionID != updateModel.EquipmentDefinitionID)
                return "The new equipment definition should match the old one.";

            // Check if children are valid and if the 2 level hierarchy is not violated
            // (Equipment's children can't have children)
            StringBuilder stringBuilder = new StringBuilder("");
            foreach(var child in viewModel.Children)
            {
                string validationResult = await Validate(unitOfWork, child);
                if (validationResult != null)
                    stringBuilder.Append(validationResult + Environment.NewLine);

                var childEquipment = (await unitOfWork.Equipments
                    .Find<Data.Entities.EquipmentEntities.Equipment>(
                        where: q => q.Id == child.Id,
                        include: q => q.Include(q => q.Children)))
                    .FirstOrDefault();

                if(childEquipment != null && !childEquipment.Children.IsNullOrEmpty())
                {
                    stringBuilder.Append($"2 level hierarchy violation: {childEquipment.GetIdentified()} can not be " +
                        $"a child because it is a parent for other children" + Environment.NewLine);
                }
            }

            return (stringBuilder.ToString() == "") ? null : stringBuilder.ToString();
        }

        public static AddEquipmentViewModel FromModel(Data.Entities.EquipmentEntities.Equipment model)
        {
            return new AddEquipmentViewModel()
            {
                Id = model.Id,
                Children = model.Children == null
                    ? null
                    : model.Children.Select(q => AddEquipmentViewModel.FromModel(q)).ToList(),
                Currency = model.Currency,
                Description = model.Description,
                EquipmentDefinitionID = model.EquipmentDefinitionID,
                IsDefect = model.IsDefect,
                IsUsed = model.IsUsed,
                Price = model.Price ?? 0,
                PurchaseDate = model.PurchaseDate ?? DateTime.MinValue,
                SerialNumber = model.SerialNumber,
                Temsid = model.TEMSID
            };
        }
    }
}
