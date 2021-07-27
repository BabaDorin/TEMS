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
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.Key;

namespace temsAPI.Controllers.KeyControllers
{
    public class KeyController : TEMSController
    {
        private KeyManager _keyManager;
        public KeyController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            KeyManager keyManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _keyManager = keyManager;
        }

        [HttpPost("key/Create")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<IActionResult> Create([FromBody] AddKeyViewModel viewModel)
        {
            try
            {
                var result = await _keyManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when creating the key.", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/Archieve/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<IActionResult> Archieve(string keyId)
        {
            try
            {
                string archivationResult = await new ArchieveHelper(_unitOfWork, User)
                    .SetKeyArchivationStatus(keyId, true);
                if (archivationResult != null)
                    return ReturnResponse(archivationResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while archieving the key and it's related data", ResponseStatus.Fail);
            }
        }

        [HttpDelete("key/Remove/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<IActionResult> Remove(string keyId)
        {
            try
            {
                string result = await _keyManager.Remove(keyId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the key", ResponseStatus.Fail);
            }
        }

        [HttpDelete("key/RemoveAllocation/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<IActionResult> RemoveAllocation(string allocationId)
        {
            try
            {
                string result = await _keyManager.RemoveAllocation(allocationId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the allocation", ResponseStatus.Fail);
            }
        }

        [HttpPost("key/CreateAllocation")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<IActionResult> CreateAllocation([FromBody] AddKeyAllocation viewModel)
        {
            try
            {
                var result = await _keyManager.CreateAllocation(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when creating the allocation", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/Get")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var keys = await _keyManager.GetKeysSimplified(); 
                return Ok(keys);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching keys", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/GetAllAutocompleteOptions")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<IActionResult> GetAllAutocompleteOptions()
        {
            try
            {
                var options = await _keyManager.GetAutocompleteOptions();
                return Ok(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching keys autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/GetAllocations/{keyId}/{roomId}/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<IActionResult> GetAllocations(string keyId, string roomId, string personnelId)
        {
            try
            {
                var allocations = await _keyManager.GetAllocations(keyId, roomId, personnelId);
                return Ok(allocations);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching key allocations", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/MarkAsReturned/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<IActionResult> MarkAsReturned(string keyId)
        {
            try
            {
                string result = await _keyManager.MarkKeyAsReturned(keyId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while marking the key as returned", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/ArchieveAllocation/{allocationId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<IActionResult> ArchieveAllocation(string allocationId, bool? archivationStatus)
        {
            try
            {
                if (archivationStatus == null) archivationStatus = true;

                string archivationResult = await (new ArchieveHelper(_unitOfWork, User)
                    .SetKeyAllocationArchivationStatus(allocationId, (bool)archivationStatus));
                if (archivationResult != null)
                    return ReturnResponse(archivationResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while archieving the specified allocation", ResponseStatus.Fail);
            }
        }
    }
}
