using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Validation;
using temsAPI.ViewModels.EquipmentDefinition;

namespace temsAPI.Controllers.Equipment
{
    public class EquipmentDefinitionController : TEMSController
    {
        public EquipmentDefinitionController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
           : base(mapper, unitOfWork, userManager)
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Insert([FromBody] AddEquipmentDefinitionViewModel viewModel)
        {
            // Identifier is required
            if (String.IsNullOrEmpty((viewModel.Identifier = viewModel.Identifier.Trim())))
                return ReturnResponse("Please provide a valid identifier", Status.Fail);

            // Definition with this identifier already exists
            if(await _unitOfWork.EquipmentDefinitions.isExists(q => q.Identifier == viewModel.Identifier))
                return ReturnResponse("There is already a definition having this identifier", Status.Fail);

            // Invalid TypeId
            if(!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == viewModel.TypeId))
                return ReturnResponse("The Equipment Type specified does not exist.", Status.Fail);

            // Invalid data for price or currency
            double price;
            if (!double.TryParse(viewModel.Price.ToString(), out price) ||
                price < 0 ||
                (new List<string>() { "lei", "eur", "usd"}).IndexOf(viewModel.Currency) == -1)
                return ReturnResponse("Invalid data provided for price or currency", Status.Fail);

            // Validating properties
            foreach (var property in viewModel.Properties)
            {
                property.Label = property.Label.Trim();

                if (!await DataTypeValidation.IsValidAsync(property, _unitOfWork))
                    return ReturnResponse("One or more properties are invalid. Please review your data", Status.Fail);
            }

            // If we got so far, it might be valid enough
            EquipmentDefinition equipmentDefinition = new EquipmentDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = viewModel.Identifier,
                EquipmentTypeID = viewModel.TypeId,
                Price = viewModel.Price,
                Currency = viewModel.Currency,
            };

            foreach(var property in viewModel.Properties)
            {
                equipmentDefinition.EquipmentSpecifications.Add(new EquipmentSpecifications
                {
                    Id = Guid.NewGuid().ToString(),
                    EquipmentDefinitionID = equipmentDefinition.Id,
                    PropertyID = (await _unitOfWork.Properties.Find(q => q.Name == property.Value)).Id,
                    Value = property.Label,
                });
            }

            // Children - Will be implemented soon

            await _unitOfWork.EquipmentDefinitions.Create(equipmentDefinition);
            await _unitOfWork.Save();

            if(!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == equipmentDefinition.Id))
                return ReturnResponse("Fail", Status.Fail);
            else
                return ReturnResponse("Success", Status.Succes);
        }
    }
}
