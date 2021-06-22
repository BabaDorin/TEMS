using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.System_Files;
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
            KeyManager keyManager) : base(mapper, unitOfWork, userManager)
        {
            _keyManager = keyManager;
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> Get()
        {
            try
            {
                var keys = await _keyManager.GetKeysSimplified(); 
                return Json(keys);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching keys", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> GetAllAutocompleteOptions()
        {
            try
            {
                var options = await _keyManager.GetAutocompleteOptions();
                return Json(options);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching keys autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> Create([FromBody] AddKeyViewModel viewModel)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the key.", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/remove/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> Remove(string keyId)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while removing the key", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> CreateAllocation([FromBody] AddKeyAllocation viewModel)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the allocation", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/getallocations/{keyId}/{roomId}/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> GetAllocations(string keyId, string roomId, string personnelId)
        {
            try
            {
                var allocations = await _keyManager.GetAllocations(keyId, roomId, personnelId);
                return Json(allocations);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching key allocations", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/markasreturned/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> MarkAsReturned(string keyId)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while marking the key as returned", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/archieve/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> Archieve(string keyId)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while archieving the key and it's related data", ResponseStatus.Fail);
            }
        }

        [HttpGet("key/archieveallocation/{allocationId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        public async Task<JsonResult> ArchieveAllocation(string allocationId, bool? archivationStatus)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while archieving the specified allocation", ResponseStatus.Fail);
            }
        }
    }
}
