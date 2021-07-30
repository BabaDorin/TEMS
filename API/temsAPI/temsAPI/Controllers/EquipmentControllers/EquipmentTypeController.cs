using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Controllers;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.EquipmentType;

namespace temsAPI.EquipmentControllers
{
    public class EquipmentTypeController : TEMSController
    {
        readonly EquipmentTypeManager _equipmentTypeManager;
        public EquipmentTypeController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentTypeManager eqTypeManager,
            ILogger<TEMSController> logger)
            : base(unitOfWork, userManager, logger)
        {
            _equipmentTypeManager = eqTypeManager;
        }

        [HttpPost("equipmenttype/Add")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while registering the type")]
        public async Task<IActionResult> Add([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            string result = await _equipmentTypeManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse($"Success", ResponseStatus.Success);
        }

        [HttpPut("equipmenttype/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while updating the type")]
        public async Task<IActionResult> Update([FromBody] AddEquipmentTypeViewModel viewModel)
        {
            string result = await _equipmentTypeManager.Update(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse($"Success", ResponseStatus.Success);
        }

        [HttpGet("equipmenttype/Archieve/{typeId}/{status}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while removing the type")]
        public async Task<IActionResult> Archieve(string typeId, bool status = true)
        {
            var type = await _equipmentTypeManager.GetById(typeId);
            if (type == null)
                return ReturnResponse("The specified type does not exist", ResponseStatus.Neutral);

            string archivationResult = await new ArchieveHelper(_unitOfWork, User)
                .SetTypeArchivationStatus(typeId, status);
            if (archivationResult != null)
                return ReturnResponse(archivationResult, ResponseStatus.Neutral);

            return ReturnResponse("Success!", ResponseStatus.Success);
        }

        [HttpDelete("equipmenttype/Remove/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the type")]
        public async Task<IActionResult> Remove(string typeId)
        {
            string result = await _equipmentTypeManager.Remove(typeId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("equipmenttype/GetAllAutocompleteOptions/{filter?}/{includeChildTypes?}")]
        [DefaultExceptionHandler("An error occured while fetching type autocomplete options")]
        public async Task<IActionResult> GetAllAutocompleteOptions(string filter, bool includeChildTypes)
        {
            var options = await _equipmentTypeManager.GetAutocompleteOptions(filter, includeChildTypes);
            return Ok(options);
        }

        [HttpGet("equipmenttype/GetSimplified")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching types")]
        public async Task<IActionResult> GetSimplified()
        {
            var simplifiedType = await _equipmentTypeManager.GetSimplified();
            return Ok(simplifiedType);
        }

        [HttpGet("equipmenttype/getsimplifiedbyid/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the type")]
        public async Task<IActionResult> GetSimplifiedById(string typeId)
        {
            var type = await _equipmentTypeManager.GetSimplifiedById(typeId);
            if (type == null)
                return ReturnResponse("Invalid id provided", ResponseStatus.Neutral);

            return Ok(type);
        }

        [HttpPost("equipmenttype/FullType")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the type")]
        public async Task<IActionResult> FullType([FromBody] string typeId)
        {
            var type = await _equipmentTypeManager.GetFullById(typeId);
            if (type == null)
                return ReturnResponse("Invalid id provided", ResponseStatus.Neutral);

            var viewModel = ViewEquipmentTypeViewModel.FromModel(type);
            return Ok(viewModel);
        }

        [HttpGet("equipmenttype/GetPropertiesOfType/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching properties.")]
        public async Task<IActionResult> GetPropertiesOfType(string typeId)
        {
            var options = await _equipmentTypeManager.GetPropertiesOfType(typeId);
            return Ok(options);
        }
    }
}
