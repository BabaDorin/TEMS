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
using temsAPI.Helpers;
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
            string validationResult = await AddEquipmentDefinitionViewModel.Validate(_unitOfWork, viewModel);
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Fail);

            // If we got so far, it might be valid enough
            var equipmentDefinition = await EquipmentDefinition.FromViewModel(_unitOfWork, viewModel);

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
                string validationResult = await AddEquipmentDefinitionViewModel.Validate(_unitOfWork, viewModel);
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

        [HttpGet("equipmentdefinition/getdefinitionsoftype/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetDefinitionsOfType(string typeId)
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

        // find a better sollution
        public class DefinitionsOfTypesModel
        {
            public string Filter { get; set; }
            public List<string> TypeIds { get; set; }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetDefinitionsOfTypes([FromBody] DefinitionsOfTypesModel viewModel)
        {
            try
            {

                Expression<Func<EquipmentDefinition, bool>> expression;

                if (viewModel == null || viewModel.TypeIds == null)
                {
                    //return Json(new List<Option>());
                    expression = q => !q.IsArchieved;
                }
                else
                {
                    if (viewModel.TypeIds == null)
                        viewModel.TypeIds = new List<string>();

                    if (viewModel.TypeIds != null && viewModel.TypeIds.Count > 0)
                        expression = q => !q.IsArchieved && viewModel.TypeIds.Contains(q.EquipmentTypeID);
                    else
                        expression = q => !q.IsArchieved;

                    if (viewModel.Filter != null)
                    {
                        Expression<Func<EquipmentDefinition, bool>> filterExpression =
                            q => q.Identifier.Contains(viewModel.Filter);

                        expression = ExpressionCombiner.CombineTwo(expression, filterExpression);
                    }
                }
                

                List<Option> resultViewModel = (await _unitOfWork.EquipmentDefinitions
                    .FindAll<Option>(
                        where: expression,
                        take: 5,
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Identifier,
                            Additional = q.EquipmentTypeID
                        }
                    )).ToList();

                return Json(resultViewModel);
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

        [HttpGet("equipmentdefinition/archieve/{definitionId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string definitionId, bool archivationStatus = true)
        {
            try
            {
                var archievingResult = await (new ArchieveHelper(_userManager, _unitOfWork))
                    .SetDefinitionArchivationStatus(definitionId, archivationStatus);

                if (archievingResult != null)
                    return ReturnResponse(archievingResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
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
