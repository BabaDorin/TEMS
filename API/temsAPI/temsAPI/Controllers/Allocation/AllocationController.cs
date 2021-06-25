using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files;
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
            EquipmentManager equipmentManager) 
            : base(mapper, unitOfWork, userManager)
        {
            _equipmentManager = equipmentManager;
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Create([FromBody] AddAllocationViewModel viewModel)
        {
            try
            {
                var result = await _equipmentManager.CreateAllocation(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when creating the allocation.", ResponseStatus.Fail);
            }
        }

        [HttpGet("allocation/markasreturned/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> MarkAsReturned(string allocationId)
        {
            try
            {
                var result = await _equipmentManager.MarkAllocationAsReturned(allocationId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while marking the allocation as returned", ResponseStatus.Fail);
            }
        }

        [HttpGet("allocation/archieve/{allocationId}/{status?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string allocationId, bool status = true)
        {
            try
            {
                string result = await _equipmentManager.ArchieveAllocation(allocationId, status);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while archieving the allocation", ResponseStatus.Fail);
            }
        }

        [HttpGet("allocation/remove/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> Remove(string allocationId)
        {
            try
            {
                string result = await _equipmentManager.RemoveAllocation(allocationId);
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

        [HttpGet("allocation/getofentity/{entityType}/{entityId}/{archieve?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetOfEntity(string entityType, string entityId)
        {
            try
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

                return Json(allocations);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching entity's allocations", ResponseStatus.Fail);
            }
        }
        
        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetAllocations([FromBody] EntityCollection entityCollection)
        {
            try
            {
                var allocations = await _equipmentManager.GetAllocations(entityCollection);
                return Json(allocations);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while retrieving allocations", ResponseStatus.Fail);
            }
        }
    }
}
