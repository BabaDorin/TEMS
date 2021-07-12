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

        [HttpPost("equipmenttype/Add")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Add([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            string result = await _equipmentTypeManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse($"Success", ResponseStatus.Success);
        }

        [HttpPut("equipmenttype/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Update([FromBody] AddEquipmentTypeViewModel viewModel)
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

        [HttpGet("equipmenttype/Archieve/{typeId}/{status}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Archieve(string typeId, bool status = true)
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

        [HttpDelete("equipmenttype/Remove/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<IActionResult> Remove(string typeId)
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

        [HttpGet("equipmenttype/GetAllAutocompleteOptions/{filter?}")]
        public async Task<IActionResult> GetAllAutocompleteOptions(string filter)
        {
            try
            {
                var options = await _equipmentTypeManager.GetAutocompleteOptions(filter);
                return Ok(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching types", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/GetSimplified")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetSimplified()
        {
            try
            {
                var simplifiedType = await _equipmentTypeManager.GetSimplified();
                return Ok(simplifiedType);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching types", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/getsimplifiedbyid/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetSimplifiedById(string typeId)
        {
            try
            {
                var type = await _equipmentTypeManager.GetSimplifiedById(typeId);
                if (type == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                return Ok(type);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching type", ResponseStatus.Fail);
            }
        }

        [HttpPost("equipmenttype/FullType")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> FullType([FromBody] string typeId)
        {
            try
            {
                var type = await _equipmentTypeManager.GetFullById(typeId);
                if (type == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                var viewModel = ViewEquipmentTypeViewModel.FromModel(type);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching type info", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipmenttype/GetPropertiesOfType/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetPropertiesOfType(string typeId)
        {
            try
            {
                var options = await _equipmentTypeManager.GetPropertiesOfType(typeId);
                return Ok(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching properties.", ResponseStatus.Fail);
            }
        }
    }
}
