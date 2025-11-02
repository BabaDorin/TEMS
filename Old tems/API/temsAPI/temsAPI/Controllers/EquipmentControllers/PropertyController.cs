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
using temsAPI.ViewModels.Property;

namespace temsAPI.EquipmentControllers
{
    public class PropertyController : TEMSController
    {
        readonly EquipmentPropertyManager _eqPropertyManager;
        public PropertyController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentPropertyManager equipmentPropertyManager,
            ILogger<TEMSController> logger) 
            : base(unitOfWork, userManager, logger)
        {
            _eqPropertyManager = equipmentPropertyManager;
        }

        [HttpPost("property/Add")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while creating the property")]
        public async Task<IActionResult> Add([FromBody] AddPropertyViewModel viewModel)
        {
            string result = await _eqPropertyManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPut("property/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when saving the property")]
        public async Task<IActionResult> Update([FromBody] AddPropertyViewModel viewModel)
        {
            var result = await _eqPropertyManager.Update(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success!", ResponseStatus.Success);
        }

        [HttpGet("property/Archieve/{propertyId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while changing the archivation status")]
        public async Task<IActionResult> Archieve(string propertyId, bool archivationStatus = true)
        {
            var archievingResult = await (new ArchieveHelper(_unitOfWork, User))
                     .SetPropertyArchivationStatus(propertyId, archivationStatus);
            if (archievingResult != null)
                return ReturnResponse(archievingResult, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("property/Remove/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the property")]
        public async Task<IActionResult> Remove(string propertyId)
        {
            string result = await _eqPropertyManager.Remove(propertyId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }
        
        [HttpGet("property/Get")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching properties")]
        public async Task<IActionResult> Get()
        {
            var options = await _eqPropertyManager.GetAutocompleteOptions();
            return Ok(options);
        }

        [HttpGet("property/GetSimplified")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching properties")]
        public async Task<IActionResult> GetSimplified()
        {
            var properties = await _eqPropertyManager.GetSimplified();
            return Ok(properties);
        }

        [HttpGet("property/GetSimplifiedById/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching the property")]
        public async Task<IActionResult> GetSimplifiedById(string propertyId)
        {
            var property = await _eqPropertyManager.GetSimplifiedById(propertyId);
            return Ok(property);
        }

        [HttpGet("property/GetById/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching the property")]
        public async Task<IActionResult> GetById(string propertyId)
        {
            var property = await _eqPropertyManager.GetFullById(propertyId);
            var viewModel = ViewPropertyViewModel.FromModel(property);
            return Ok(viewModel);
        }
    }
}
