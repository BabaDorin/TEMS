using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Controllers;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels.EquipmentType;

namespace temsAPI.EquipmentControllers
{
    public class EquipmentTypeController : TEMSController
    {
        EquipmentTypeManager _equipmentTypeManager;
        public EquipmentTypeController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentTypeManager eqTypeManager,
            ILogger<TEMSController> logger)
            : base(mapper, unitOfWork, userManager, logger)
        {
            _equipmentTypeManager = eqTypeManager;
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Add([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            string result = await _equipmentTypeManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse($"Success", ResponseStatus.Success);
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            try
            {
                string result = await _equipmentTypeManager.Update(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse($"Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when saving the type", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/archieve/{typeId}/{status}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string typeId, bool status = true)
        {
            try
            {
                // check if type exists
                var type = await _equipmentTypeManager.GetById(typeId);
                if (type == null)
                    return ReturnResponse("The specified type does not exist", ResponseStatus.Fail);

                string archivationResult = await new ArchieveHelper(_unitOfWork, User)
                    .SetTypeArchivationStatus(typeId, status);
                if (archivationResult != null)
                    return ReturnResponse(archivationResult, ResponseStatus.Fail);

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when removing the type", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/remove/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> Remove(string typeId)
        {
            try
            {
                string result = await _equipmentTypeManager.Remove(typeId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the type.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/getallautocompleteoptions/{filter?}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(string filter)
        {
            try
            {
                var options = await _equipmentTypeManager.GetAutocompleteOptions(filter);
                return Json(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching types", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetSimplified()
        {
            try
            {
                var simplifiedType = await _equipmentTypeManager.GetSimplified();
                return Json(simplifiedType);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching types", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/getsimplifiedbyid/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetSimplifiedById(string typeId)
        {
            try
            {
                var type = await _equipmentTypeManager.GetSimplifiedById(typeId);
                if (type == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                return Json(type);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching type", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> FullType([FromBody] string typeId)
        {
            try
            {
                var type = await _equipmentTypeManager.GetFullById(typeId);
                if (type == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                var viewModel = ViewEquipmentTypeViewModel.FromModel(type);
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching type info", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/getpropertiesoftype/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetPropertiesOfType(string typeId)
        {
            try
            {
                var options = await _equipmentTypeManager.GetPropertiesOfType(typeId);
                return Json(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching properties.", ResponseStatus.Fail);
            }
        }
    }
}
