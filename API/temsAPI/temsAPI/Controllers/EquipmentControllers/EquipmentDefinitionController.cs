using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.Validation;
using temsAPI.ViewModels;
using temsAPI.ViewModels.EquipmentDefinition;
using temsAPI.ViewModels.EquipmentType;
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
        public async Task<JsonResult> Add([FromBody] AddEquipmentDefinitionViewModel viewModel)
        {
            string validationResult = await ValidateAddDefinitionViewModel(viewModel);
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Fail);

            // If we got so far, it might be valid enough
            EquipmentDefinition equipmentDefinition = new EquipmentDefinition
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = viewModel.Identifier,
                EquipmentTypeID = viewModel.TypeId,
                Price = viewModel.Price,
                Currency = viewModel.Currency,
                Description = viewModel.Description,
            };

            foreach (var property in viewModel.Properties)
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

            if (!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == equipmentDefinition.Id))
                return ReturnResponse("Fail", ResponseStatus.Fail);
            else
                return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddEquipmentDefinitionViewModel viewModel)
        {
            try
            {
                string validationResult = await ValidateAddDefinitionViewModel(viewModel);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                EquipmentDefinition definition = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinition>(
                        where: q => q.Id == viewModel.Id,
                        include: q => q.Include(q => q.EquipmentSpecifications)
                    )).FirstOrDefault();

                definition.Identifier = viewModel.Identifier;
                definition.EquipmentTypeID = viewModel.TypeId;
                definition.EquipmentTypeID = viewModel.TypeId;
                definition.Price = viewModel.Price;
                definition.Currency = viewModel.Currency;
                definition.Description = viewModel.Description;

                await AssignSpecifications(definition, viewModel);
                await _unitOfWork.Save();

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while updating the definition", ResponseStatus.Fail);
            }
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

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetDefinitionsOfTypes([FromBody] List<string> typeIds)
        {
            try
            {
                Expression<Func<EquipmentDefinition, bool>> expression = null;
                if (typeIds != null && typeIds.Count > 0)
                    expression = q => !q.IsArchieved && typeIds.Contains(q.EquipmentTypeID);
                else
                    expression = q => !q.IsArchieved;

                List<Option> viewModel = (await _unitOfWork.EquipmentDefinitions
                    .FindAll<Option>(
                        where: expression,
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Identifier,
                            Additional = q.Description
                        }
                    )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching definitions", ResponseStatus.Fail);
            }
        }



        [HttpGet("equipmentdefinition/getdefinitionsautocompleteoptions")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]

        [HttpGet("equipmentdefinition/getsimplified")]
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

        [HttpGet("equipmentdefinition/getsimplifiedbyid/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplifiedById(string definitionId)
        {
            try
            {
                ViewEquipmentDefinitionSimplifiedViewModel viewModel =
                    (await _unitOfWork.EquipmentDefinitions
                    .Find<ViewEquipmentDefinitionSimplifiedViewModel>(
                        where: q => q.Id == definitionId,
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
                    )).FirstOrDefault();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching the definition", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/getdefinitiontoupdate/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetDefinitionToUpdate(string definitionId)
        {
            try
            {
                var definition = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinition>(
                        where: q => q.Id == definitionId,
                        include: q => q
                        .Include(q => q.Children.Where(q => !q.IsArchieved))
                        .Include(q => q.Parent)
                        .Include(q => q.EquipmentType)
                        .Include(q => q.EquipmentSpecifications).ThenInclude(q => q.Property)))
                    .FirstOrDefault();

                if (definition == null)
                    return ReturnResponse("Invalid definition id provided", ResponseStatus.Fail);

                var viewModel = DefinitionToAddDefinition(definition);

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching the definition", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/getfulldefinition/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetFullDefinition(string definitionId)
        {
            try
            {
                // Invalid definitionId
                if (!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == definitionId))
                    return ReturnResponse("There is no definition having the specified id", ResponseStatus.Fail);

                EquipmentDefinitionViewModel viewModel = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinitionViewModel>(
                        where: q => q.Id == definitionId,
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
                            EquipmentType = new ViewEquipmentTypeViewModel
                            {
                                Id = q.EquipmentType.Id,
                                Name = q.EquipmentType.Name
                            },
                            Properties = q.EquipmentSpecifications
                            .Select(q => new ViewPropertyViewModel
                            {
                                Id = q.Property.Id,
                                DisplayName = q.Property.DisplayName,
                                Value = q.Value,
                            })
                            .ToList()
                        }))
                        .FirstOrDefault();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

        // --------------------------------------------------------------

        private static ViewEquipmentTypeViewModel EquipmentTypeToEquipmentViewModel(EquipmentType type)
        {
            return new ViewEquipmentTypeViewModel
            {
                Id = type.Id,
                Name = type.Name,
            };
        }

        /// <summary>
        /// Converts an instance of EquipmentDefinition to an instance of AddDefinitionViewModel.
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        private AddEquipmentDefinitionViewModel DefinitionToAddDefinition(EquipmentDefinition definition)
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
            {
                viewModel.Children.Add(DefinitionToAddDefinition(item));
            }

            return viewModel;
        }

        /// <summary>
        /// Validates an instance of DefinitionViewModel. If everythink is ok, it returns null, otherwise - 
        /// the error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<string> ValidateAddDefinitionViewModel(AddEquipmentDefinitionViewModel viewModel)
        {
            // If it's the update case, we make sure the specified id exists
            // And also, the equipment type should match
            if (viewModel.Id != null)
            {
                var definition = (await _unitOfWork.EquipmentDefinitions
                    .Find<EquipmentDefinition>(q => q.Id == viewModel.Id))
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
                if (await _unitOfWork.EquipmentDefinitions
                    .isExists(q => q.Identifier == viewModel.Identifier && !q.IsArchieved))
                    return "There is already a definition having this identifier";

            // Invalid TypeId
            if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == viewModel.TypeId))
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

                if (!await DataTypeValidation.IsValidAsync(property, _unitOfWork))
                    return "One or more properties are invalid. Please review your data";
            }

            return null;
        }

        /// <summary>
        /// Assigns values for definition's properties according to the data being provided by the
        /// DefinitionViewModel.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task AssignSpecifications(EquipmentDefinition model, AddEquipmentDefinitionViewModel viewModel)
        {
            var specifications = model.EquipmentSpecifications?.ToList();
            foreach (var item in specifications)
            {
                model.EquipmentSpecifications.Remove(item);
            }

            foreach (var property in viewModel.Properties)
            {
                model.EquipmentSpecifications.Add(new EquipmentSpecifications
                {
                    Id = Guid.NewGuid().ToString(),
                    EquipmentDefinitionID = model.Id,
                    PropertyID = (await _unitOfWork.Properties.Find<Property>(q => q.Name == property.Value))
                        .FirstOrDefault().Id,
                    Value = property.Label,
                });
            }
        }
    }
}
