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
using temsAPI.ViewModels.Property;

namespace temsAPI.EquipmentControllers
{
    public class PropertyController : TEMSController
    {
        EquipmentPropertyManager _eqPropertyManager;
        public PropertyController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentPropertyManager equipmentPropertyManager,
            ILogger<TEMSController> logger) 
            : base(mapper, unitOfWork, userManager, logger)
        {
            _eqPropertyManager = equipmentPropertyManager;
        }

        [HttpPost("property/Add")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Add([FromBody] AddPropertyViewModel viewModel)
        {
            try
            {
                string result = await _eqPropertyManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while creating the property", ResponseStatus.Fail);
            }
        }

        [HttpPut("property/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Update([FromBody] AddPropertyViewModel viewModel)
        {
            try
            {
                var result = await _eqPropertyManager.Update(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when saving the property", ResponseStatus.Fail);
            }
        }

        [HttpGet("property/Archieve/{propertyId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Archieve(string propertyId, bool archivationStatus = true)
        {
            try
            {
                var archievingResult = await (new ArchieveHelper(_unitOfWork, User))
                     .SetPropertyArchivationStatus(propertyId, archivationStatus);
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

        [HttpDelete("property/Remove/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<IActionResult> Remove(string propertyId)
        {
            try
            {
                string result = await _eqPropertyManager.Remove(propertyId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the property.", ResponseStatus.Fail);
            }
        }
        
        [HttpGet("property/Get")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var options = await _eqPropertyManager.GetAutocompleteOptions();
                return Ok(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching properties", ResponseStatus.Fail);
            }
        }

        [HttpGet("property/GetSimplified")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetSimplified()
        {
            try
            {
                var properties = await _eqPropertyManager.GetSimplified();
                return Ok(properties);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching properties", ResponseStatus.Fail);
            }
        }

        [HttpGet("property/GetSimplifiedById/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetSimplifiedById(string propertyId)
        {
            try
            {
                var property = await _eqPropertyManager.GetSimplifiedById(propertyId);
                return Ok(property);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching the property", ResponseStatus.Fail);
            }
        }

        [HttpGet("property/GetById/{propertyId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetById(string propertyId)
        {
            try
            {
                var property = await _eqPropertyManager.GetFullById(propertyId);
                var viewModel = ViewPropertyViewModel.FromModel(property);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching the property", ResponseStatus.Fail);
            }
        }
    }
}
