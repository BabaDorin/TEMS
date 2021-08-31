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
using temsAPI.Helpers.Filters;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.Allocation;
using static temsAPI.Data.Managers.EquipmentManager;

namespace temsAPI.Controllers.Allocation
{
    public class AllocationController : TEMSController
    {
        readonly EquipmentManager _equipmentManager;

        public AllocationController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentManager equipmentManager,
            ILogger<TEMSController> logger) 
            : base(unitOfWork, userManager, logger)
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
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("allocation/Remove/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the allocation")]
        public async Task<IActionResult> Remove(string allocationId)
        {
            string result = await _equipmentManager.RemoveAllocation(allocationId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("allocation/Archieve/{allocationId}/{status?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while archieving the allocation")]
        public async Task<IActionResult> Archieve(string allocationId, bool status = true)
        {
            string result = await _equipmentManager.ArchieveAllocation(allocationId, status);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("allocation/MarkAsReturned/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while marking the allocation as returned")]
        public async Task<IActionResult> MarkAsReturned(string allocationId)
        {
            var result = await _equipmentManager.MarkAllocationAsReturned(allocationId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost("allocation/GetAllocations")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while retrieving allocations")]
        public async Task<IActionResult> GetAllocations([FromBody] AllocationFilter filter)
        {
            var allocations = await _equipmentManager.GetAllocations(filter);
            return Ok(allocations);
        }

        [HttpPost("allocation/GetTotalItems")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while retrieving total allocations number")]
        public async Task<IActionResult> GetTotalItems([FromBody] AllocationFilter filter)
        {
            var totalItems = await _equipmentManager.GetTotalItems(filter);
            return Ok(totalItems);
        }
    }
}
