using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.Allocation;
using static temsAPI.Data.Managers.EquipmentManager;

namespace temsAPI.Controllers.Allocation
{
    public class AllocationController : TEMSController
    {
        private EquipmentManager _equipmentManager;

        public AllocationController(
            IMapper mapper, 
            IUnitOfWork 
            unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentManager equipmentManager,
            ILogger<TEMSController> logger) 
            : base(mapper, unitOfWork, userManager, logger)
        {
            _equipmentManager = equipmentManager;
        }

        [HttpPost("allocation/Create")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when creating the allocation.")]
        public async Task<IActionResult> Create([FromBody] AddAllocationViewModel viewModel)
        {
            var result = await _equipmentManager.CreateAllocation(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("allocation/Remove/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the allocation")]
        public async Task<IActionResult> Remove(string allocationId)
        {
            string result = await _equipmentManager.RemoveAllocation(allocationId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("allocation/Archieve/{allocationId}/{status?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while archieving the allocation")]
        public async Task<IActionResult> Archieve(string allocationId, bool status = true)
        {
            string result = await _equipmentManager.ArchieveAllocation(allocationId, status);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("allocation/MarkAsReturned/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while marking the allocation as returned")]
        public async Task<IActionResult> MarkAsReturned(string allocationId)
        {
            var result = await _equipmentManager.MarkAllocationAsReturned(allocationId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("allocation/GetOfEntity/{entityType}/{entityId}/{archieve?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching entity's allocations")]
        public async Task<IActionResult> GetOfEntity(string entityType, string entityId)
        {
            // Invalid identityType
            if ((new List<string>() { "any", "equipment", "room", "personnel" }).IndexOf(entityType) == -1)
                return ReturnResponse("Invalid entity type or id provided", ResponseStatus.Fail);

            // No entity id provided
            if (String.IsNullOrEmpty(entityId.Trim()))
                return ReturnResponse($"You have to provide a valid {entityType} Id", ResponseStatus.Fail);

            List<ViewAllocationSimplifiedViewModel> allocations = new();
            switch (entityType)
            {
                case "equipment":
                    allocations = await _equipmentManager.GetEquipmentAllocations(entityId);
                    break;
                case "room":
                    allocations = await _equipmentManager.GetRoomAllocations(entityId);
                    break;

                case "personnel":
                    allocations = await _equipmentManager.GetPersonnelAllocations(entityId);
                    break;
            }

            return Ok(allocations);
        }
        
        [HttpPost("allocation/GetAllocations")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while retrieving allocations")]
        public async Task<IActionResult> GetAllocations([FromBody] EntityCollection entityCollection)
        {
            var allocations = await _equipmentManager.GetAllocations(entityCollection);
            return Ok(allocations);
        }

        [HttpPost("allocation/GetTotalItems")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while retrieving total allocations number")]
        public async Task<IActionResult> GetTotalItems([FromBody] EntityCollection entityCollection)
        {
            var totalItems = await _equipmentManager.GetTotalItems(entityCollection);
            return Ok(totalItems);
        }
    }
}
