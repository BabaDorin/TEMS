using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels.EquipmentDefinition;

namespace temsAPI.Controllers.EquipmentControllers
{
    public class EquipmentDefinitionController : TEMSController
    {
        private EquipmentDefinitionManager _equipmentDefinitionManager;
        public EquipmentDefinitionController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentDefinitionManager equipmentDefinitionManager,
            ILogger<TEMSController> logger)
           : base(mapper, unitOfWork, userManager, logger)
        {
            _equipmentDefinitionManager = equipmentDefinitionManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Add([FromBody] AddEquipmentDefinitionViewModel viewModel)
        {
            try
            {
                string result = await _equipmentDefinitionManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while adding the definition", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddEquipmentDefinitionViewModel viewModel)
        {
            try
            {
                var result = await _equipmentDefinitionManager.Update(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while updating the definition", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/getdefinitionsoftype/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetDefinitionsOfType(string typeId)
        {
            try
            {
                var options = await _equipmentDefinitionManager.GetOfType(typeId);
                return Json(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("Unknown error occured when fetching definitions of specified type", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetDefinitionsOfTypes([FromBody] DefinitionsOfTypesModel filter)
        {
            try
            {
                var options = await _equipmentDefinitionManager.GetOfTypes(filter);
                return Json(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching definitions of specified types", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/getsimplified")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetSimplified()
        {
            try
            {
                var definitions = await _equipmentDefinitionManager.GetSimplified();
                return Json(definitions);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("Unknown error occured when fetching definitions", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/getsimplifiedbyid/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetSimplifiedById(string definitionId)
        {
            try
            {
                var definition = await _equipmentDefinitionManager.GetSimplifiedById(definitionId);
                return Json(definition);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching the definition", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/getdefinitiontoupdate/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetDefinitionToUpdate(string definitionId)
        {
            try
            {
                var definition = await _equipmentDefinitionManager.GetFullById(definitionId);
                if (definition == null)
                    return ReturnResponse("Invalid definition id provided", ResponseStatus.Fail);

                var viewModel = AddEquipmentDefinitionViewModel.FromModel(definition);
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching the definition", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/getfulldefinition/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetFullDefinition(string definitionId)
        {
            try
            {
                var definition = await _equipmentDefinitionManager.GetFullById(definitionId);
                if(definition == null)
                    return ReturnResponse("Invalid definition Id", ResponseStatus.Fail);

                var viewModel = EquipmentDefinitionViewModel.FromModel(definition);
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching defintion's data", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/archieve/{definitionId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string definitionId, bool archivationStatus = true)
        {
            try
            {
                var archievingResult = await (new ArchieveHelper(_unitOfWork, User))
                    .SetDefinitionArchivationStatus(definitionId, archivationStatus);

                if (archievingResult != null)
                    return ReturnResponse(archievingResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmentdefinition/remove/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> Remove(string definitionId)
        {
            try
            {
                string result = await _equipmentDefinitionManager.Remove(definitionId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the definition", ResponseStatus.Fail);
            }
        }
    }
}
