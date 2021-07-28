using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        [DefaultExceptionHandler("An error occured while creating the key.")]
        public async Task<IActionResult> Create([FromBody] AddKeyViewModel viewModel)
        {
            var result = await _keyManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("key/Archieve/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        [DefaultExceptionHandler("An error occured while archieving the key and it's related data")]
        public async Task<IActionResult> Archieve(string keyId)
        {
            string archivationResult = await new ArchieveHelper(_unitOfWork, User)
                    .SetKeyArchivationStatus(keyId, true);
            if (archivationResult != null)
                return ReturnResponse(archivationResult, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("key/Remove/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the key")]
        public async Task<IActionResult> Remove(string keyId)
        {
            string result = await _keyManager.Remove(keyId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("key/RemoveAllocation/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the allocation")]
        public async Task<IActionResult> RemoveAllocation(string allocationId)
        {
            string result = await _keyManager.RemoveAllocation(allocationId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost("key/CreateAllocation")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        [DefaultExceptionHandler("An error occured while creating the allocation")]
        public async Task<IActionResult> CreateAllocation([FromBody] AddKeyAllocation viewModel)
        {
            var result = await _keyManager.CreateAllocation(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("key/Get")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        [DefaultExceptionHandler("An error occured while fetching keys")]
        public async Task<IActionResult> Get()
        {
            var keys = await _keyManager.GetKeysSimplified();
            return Ok(keys);
        }

        [HttpGet("key/GetAllAutocompleteOptions")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        [DefaultExceptionHandler("An error occured while fetching keys autocomplete options")]
        public async Task<IActionResult> GetAllAutocompleteOptions()
        {
            var options = await _keyManager.GetAutocompleteOptions();
            return Ok(options);
        }

        [HttpGet("key/GetAllocations/{keyId}/{roomId}/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_ALLOCATE_KEYS)]
        [DefaultExceptionHandler("An error occured while fetching key allocations")]
        public async Task<IActionResult> GetAllocations(string keyId, string roomId, string personnelId)
        {
            var allocations = await _keyManager.GetAllocations(keyId, roomId, personnelId);
            return Ok(allocations);
        }

        [HttpGet("key/MarkAsReturned/{keyId}")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        [DefaultExceptionHandler("An error occured while marking the key as returned")]
        public async Task<IActionResult> MarkAsReturned(string keyId)
        {
            string result = await _keyManager.MarkKeyAsReturned(keyId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("key/ArchieveAllocation/{allocationId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_ALLOCATE_KEYS)]
        [DefaultExceptionHandler("An error occured while archieving the specified allocation")]
        public async Task<IActionResult> ArchieveAllocation(string allocationId, bool? archivationStatus)
        {
            if (archivationStatus == null) archivationStatus = true;

            string archivationResult = await (new ArchieveHelper(_unitOfWork, User)
                .SetKeyAllocationArchivationStatus(allocationId, (bool)archivationStatus));
            if (archivationResult != null)
                return ReturnResponse(archivationResult, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }
    }
}
