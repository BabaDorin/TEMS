using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.Validation;
using temsAPI.ViewModels;
using temsAPI.ViewModels.EquipmentDefinition;
using temsAPI.ViewModels.Property;

namespace temsAPI.Controllers.EquipmentControllers
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
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Insert([FromBody] AddEquipmentDefinitionViewModel viewModel)
        {
            // Identifier is required
            if (String.IsNullOrEmpty((viewModel.Identifier = viewModel.Identifier.Trim())))
                return ReturnResponse("Please provide a valid identifier", ResponseStatus.Fail);

            // Definition with this identifier already exists
            if(await _unitOfWork.EquipmentDefinitions
                .isExists(q => q.Identifier == viewModel.Identifier && !q.IsArchieved))
                return ReturnResponse("There is already a definition having this identifier", ResponseStatus.Fail);

            // Invalid TypeId
            if(!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == viewModel.TypeId))
                return ReturnResponse("The Equipment Type specified does not exist.", ResponseStatus.Fail);

            // Invalid data for price or currency
            double price;
            if (!double.TryParse(viewModel.Price.ToString(), out price) ||
                price < 0 ||
                (new List<string>() { "lei", "eur", "usd"}).IndexOf(viewModel.Currency) == -1)
                return ReturnResponse("Invalid data provided for price or currency", ResponseStatus.Fail);

            // Validating properties
            foreach (var property in viewModel.Properties)
            {
                property.Label = property.Label.Trim();

                if (!await DataTypeValidation.IsValidAsync(property, _unitOfWork))
                    return ReturnResponse("One or more properties are invalid. Please review your data", ResponseStatus.Fail);
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
                    PropertyID = (await _unitOfWork.Properties.Find<Property>(q => q.Name == property.Value))
                        .FirstOrDefault().Id,
                    Value = property.Label,
                });
            }

            // Children - Will be implemented soon

            await _unitOfWork.EquipmentDefinitions.Create(equipmentDefinition);
            await _unitOfWork.Save();

            if(!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == equipmentDefinition.Id))
                return ReturnResponse("Fail", ResponseStatus.Fail);
            else
                return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetDefinitionsOfType([FromBody] string typeId)
        {
            List<Option> options = new List<Option>();
            try
            {
                (await _unitOfWork.EquipmentDefinitions
                    .FindAll<EquipmentDefinition>(q => q.EquipmentTypeID == typeId && !q.IsArchieved))
                .ToList()
                .ForEach(q => options.Add(new Option
                {
                    Value = q.Id,
                    Label = q.Identifier
                }));

                return Json(options);
            }
            catch (Exception)
            {
                return ReturnResponse("Unknown error occured when fetching definitions", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplified()
        {
            try
            {
                List<ViewEquipmentDefinitionSimplifiedViewModel> viewModel =
                    (await _unitOfWork.EquipmentDefinitions
                    .FindAll<ViewEquipmentDefinitionSimplifiedViewModel>(
                        where: q => !q.IsArchieved,
                        include: q => q
                        .Include(q => q.Parent)
                        .Include(q => q.Children.Where(q => !q.IsArchieved))
                        .Include(q => q.EquipmentType),
                        select: q => new ViewEquipmentDefinitionSimplifiedViewModel
                        {
                            Id = q.Id,
                            Identifier = q.Identifier,
                            Parent = q.Parent != null
                                ? q.Parent.Identifier
                                : null,
                            Children = String.Join(", ", q.Children
                                .Where(q => !q.IsArchieved).Select(q => q.Identifier)),
                            EquipmentType = q.EquipmentType.Name
                        }
                    )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("Unknown error occured when fetching definitions", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetFullDefinition([FromBody] string definitionId)
        {
            try
            {
                // Invalid definitionId
                if (!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == definitionId))
                    return ReturnResponse("There is not definition having the specified id", ResponseStatus.Fail);

                EquipmentDefinitionViewModel viewModel = new EquipmentDefinitionViewModel();
                EquipmentDefinition model = (await _unitOfWork.EquipmentDefinitions.Find<EquipmentDefinition>(q => q.Id == definitionId,
                        include: q => q
                                .Include(q => q.Children.Where(q => !q.IsArchieved))
                                .Include(q => q.EquipmentSpecifications)
                                .ThenInclude(q => q.Property).ThenInclude(q => q.DataType)
                                .Include(q => q.Parent)
                                .Include(q => q.EquipmentType)))
                                .FirstOrDefault();

                viewModel = _mapper.Map<EquipmentDefinitionViewModel>(model);

                foreach (var eqspec in model.EquipmentSpecifications)
                {
                    viewModel.EquipmentType.Properties.Add(_mapper.Map<ViewPropertyViewModel>(eqspec.Property));
                }

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                return ReturnResponse("Unknown error occured when fetching the full definition, " + ex.Message, ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/remove/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Remove(string definitionId)
        {
            try
            {
                var definition = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinition>(q => q.Id == definitionId))
                    .FirstOrDefault();

                if (definition == null)
                    return ReturnResponse("Invalid definition id provided", ResponseStatus.Fail);

                definition.IsArchieved = true;
                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while deleting the specified definition", ResponseStatus.Fail);
            }
        }

    }
}
