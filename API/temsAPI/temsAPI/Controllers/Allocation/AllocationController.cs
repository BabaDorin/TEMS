using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Any;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Allocation;

namespace temsAPI.Controllers.Allocation
{
    public class AllocationController : TEMSController
    {
        public AllocationController(
            IMapper mapper, 
            IUnitOfWork 
            unitOfWork, 
            UserManager<TEMSUser> userManager) 
            : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Create([FromBody] AddAllocationViewModel viewModel)
        {
            try
            {
                // Invalid equipments provided
                foreach (Option equipment in viewModel.Equipments)
                    if (!await _unitOfWork.Equipments.isExists(q => q.Id == equipment.Value))
                        return ReturnResponse("One or more equipments are invalid.", ResponseStatus.Fail);

                // No allocation type provided
                if ((new List<string> { "personnel", "room" }).IndexOf(viewModel.AllocateToType) == -1)
                    return ReturnResponse("Invalid type provided", ResponseStatus.Fail);

                // No allocation id provided or the provided one is invalid
                if (String.IsNullOrEmpty(viewModel.AllocateToType))
                    return ReturnResponse("Please, provide a valid allocation object type", ResponseStatus.Fail);

                List<string> equipmentsWhereFailed = new List<string>();
                if (viewModel.AllocateToType == "personnel")
                {
                    if (!await _unitOfWork.Personnel.isExists(q => q.Id == viewModel.AllocateToId))
                        return ReturnResponse("Allocatee id seems invalid.", ResponseStatus.Fail);

                    foreach (Option equipment in viewModel.Equipments)
                    {
                        await ClosePreviousAllocations(equipment.Value);

                        var model = new EquipmentAllocation
                        {
                            Id = Guid.NewGuid().ToString(),
                            DateAllocated = DateTime.Now,
                            EquipmentID = equipment.Value,
                            PersonnelID = viewModel.AllocateToId
                        };

                        await _unitOfWork.EquipmentAllocations.Create(model);
                        await _unitOfWork.Save();

                        if (!await _unitOfWork.EquipmentAllocations.isExists(q => q.Id == model.Id))
                            equipmentsWhereFailed.Add(equipment.Label);
                    }
                }


                if (viewModel.AllocateToType == "room")
                {
                    if (!await _unitOfWork.Rooms.isExists(q => q.Id == viewModel.AllocateToId))
                        return ReturnResponse("Allocatee id seems invalid.", ResponseStatus.Fail);

                    foreach (Option equipment in viewModel.Equipments)
                    {
                        await ClosePreviousAllocations(equipment.Value);

                        var model = new EquipmentAllocation
                        {
                            Id = Guid.NewGuid().ToString(),
                            DateAllocated = DateTime.Now,
                            EquipmentID = equipment.Value,
                            RoomID = viewModel.AllocateToId
                        };

                        await _unitOfWork.EquipmentAllocations.Create(model);
                        await _unitOfWork.Save();

                        if (!await _unitOfWork.EquipmentAllocations.isExists(q => q.Id == model.Id))
                            equipmentsWhereFailed.Add(equipment.Label);
                    }
                }

                if (equipmentsWhereFailed.Count == 0)
                    return ReturnResponse("Success!", ResponseStatus.Success);
                else
                    return ReturnResponse(
                        "The following equipments have not been allocated due to unhandled error:" 
                        + string.Join(",", equipmentsWhereFailed),
                        ResponseStatus.Fail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the allocation.", ResponseStatus.Fail);
            }
        }


        [HttpGet("allocation/markasreturned/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> MarkAsReturned(string allocationId)
        {
            try
            {
                var allocation = (await _unitOfWork.EquipmentAllocations
                    .Find<EquipmentAllocation>(q => q.Id == allocationId))
                    .FirstOrDefault();

                if (allocation == null)
                    return ReturnResponse("Invalid allocation provided", ResponseStatus.Fail);

                allocation.DateReturned = DateTime.Now;
                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while returning the allocation", ResponseStatus.Fail);
            }
        }

        [HttpGet("allocation/remove/{allocationId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Remove(string allocationId)
        {
            try
            {
                var allocation = (await _unitOfWork.EquipmentAllocations
                    .Find<EquipmentAllocation>(q => q.Id == allocationId))
                    .FirstOrDefault();

                if (allocation == null)
                    return ReturnResponse("Invalid allocation provided", ResponseStatus.Fail);

                allocation.IsArchieved = true;
                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while returning the allocation", ResponseStatus.Fail);
            }
        }
        [HttpGet("allocation/getofentity/{entityType}/{entityId}/{archieve?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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

                //if (archieve == null) archieve = false;
                //Expression<Func<EquipmentAllocation, bool>> archExpression = q => q.IsArchieved == (bool)archieve;

                Expression<Func<EquipmentAllocation, bool>> expression = null;

                IUnitOfWork entityUnitOfWork = null;
                switch (entityType)
                {
                    case "equipment":
                        entityUnitOfWork = (IUnitOfWork)_unitOfWork.Equipments;
                        if (!await _unitOfWork.Equipments.isExists(q => q.Id == entityId))
                            return ReturnResponse("Invalid entity type or id provided", ResponseStatus.Fail);

                        expression = q => q.EquipmentID == entityId;
                        break;

                    case "room":
                        if (!await _unitOfWork.Rooms.isExists(q => q.Id == entityId))
                            return ReturnResponse("Invalid entity type or id provided", ResponseStatus.Fail);

                        expression = q => q.RoomID == entityId;
                        break;

                    case "personnel":
                        if (!await _unitOfWork.Personnel.isExists(q => q.Id == entityId))
                            return ReturnResponse("Invalid entity type or id provided", ResponseStatus.Fail);

                        expression = q => q.PersonnelID == entityId;
                        break;
                }

                //expression = ExpressionCombiner.CombineTwo(expression, archExpression);

                List<ViewAllocationSimplifiedViewModel> viewModel = (await _unitOfWork.EquipmentAllocations
                    .FindAll<ViewAllocationSimplifiedViewModel>(
                        where: expression,
                        include: q => q.Include(q => q.Room)
                                       .Include(q => q.Personnel)
                                       .Include(q => q.Equipment).ThenInclude(q => q.EquipmentDefinition),
                        select: q => new ViewAllocationSimplifiedViewModel
                        {
                            Id = q.Id,
                            DateAllocated = q.DateAllocated,
                            DateReturned = q.DateReturned,
                            Equipment = new Option
                            {
                                Value = q.Equipment.Id,
                                Label = q.Equipment.TemsIdOrSerialNumber,
                                Additional = q.Equipment.EquipmentDefinition.Identifier
                            },
                            Personnel = (q.Personnel == null)
                                ? null
                                : new Option
                                {
                                    Value = q.Personnel.Id,
                                    Label = q.Personnel.Name,
                                },
                            Room = (q.Room == null)
                                ? null
                                : new Option
                                {
                                    Value = q.Room.Id,
                                    Label = q.Room.Identifier,
                                },
                        })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching entity's allocations", ResponseStatus.Fail);
            }
        }

        /*
         equipmentIds: equipmentIds,
          definitionIds: definitionIds,
          personnelIds: personnelIds,
          roomIds: roomIds,*/
        public class EntityCollection
        {
            public List<string> EquipmentIds { get; set; }
            public List<string> DefinitionIds { get; set; }
            public List<string> PersonnelIds { get; set; }
            public List<string> RoomIds { get; set; }
            //public int PageNumber { get; set; } = 1;
            //public int ItemsPerPage { get; set; } = 30;
            public string Include { get; set; }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetAllocations([FromBody] EntityCollection entityCollection)
        {
            try
            {
                Expression<Func<EquipmentAllocation, bool>> equipmentExpression = null;
                if (entityCollection.EquipmentIds != null && entityCollection.EquipmentIds.Count > 0)
                    equipmentExpression = q => entityCollection.EquipmentIds.Contains(q.EquipmentID);

                Expression<Func<EquipmentAllocation, bool>> definitionsExpression = null;
                if (entityCollection.DefinitionIds != null && entityCollection.DefinitionIds.Count > 0)
                    definitionsExpression = q => entityCollection.DefinitionIds.Contains(q.Equipment.EquipmentDefinitionID);

                Expression<Func<EquipmentAllocation, bool>> roomExpression = null;
                if (entityCollection.RoomIds != null && entityCollection.RoomIds.Count > 0)
                    roomExpression = q => entityCollection.RoomIds.Contains(q.RoomID);

                Expression<Func<EquipmentAllocation, bool>> personnelExpression = null;
                if (entityCollection.PersonnelIds != null && entityCollection.PersonnelIds.Count > 0)
                    personnelExpression = q => entityCollection.PersonnelIds.Contains(q.PersonnelID);

                Expression<Func<EquipmentAllocation, bool>> stateExpression = null;
                switch (entityCollection.Include)
                {
                    case "active": stateExpression = q => q.DateReturned == null; break;
                    case "returned": stateExpression = q => q.DateReturned != null; break;
                }

                Expression<Func<EquipmentAllocation, bool>> finalExpression = 
                    ExpressionCombiner.And(
                        equipmentExpression, 
                        definitionsExpression,
                        roomExpression,
                        personnelExpression,
                        stateExpression);

                Func<IQueryable<EquipmentAllocation>, IOrderedQueryable<EquipmentAllocation>> orderByExp =
                    (entityCollection.Include == "returned")
                    ? q => q.OrderByDescending(q => q.DateReturned)
                    : q => q.OrderByDescending(q => q.DateAllocated);

                //if (entityCollection.PageNumber < 1 || entityCollection.ItemsPerPage < 1)
                //    return ReturnResponse("Invalid page number of items per page", ResponseStatus.Fail);

                //int skip = entityCollection.PageNumber - 1 * entityCollection.ItemsPerPage;
                //int take = entityCollection.ItemsPerPage;

                var viewModel = (await _unitOfWork.EquipmentAllocations
                    .FindAll<ViewAllocationSimplifiedViewModel>(
                        include: q => q.Include(q => q.Room)
                                       .Include(q => q.Personnel)
                                       .Include(q => q.Equipment).ThenInclude(q => q.EquipmentDefinition),
                        where: finalExpression,
                        orderBy: orderByExp,
                        //skip: skip,
                        //take: take,
                        select: q => new ViewAllocationSimplifiedViewModel
                        {
                            Id = q.Id,
                            DateAllocated = q.DateAllocated,
                            DateReturned = q.DateReturned,
                            Equipment = new Option
                            {
                                Value = q.Equipment.Id,
                                Label = q.Equipment.TemsIdOrSerialNumber,
                                Additional = q.Equipment.EquipmentDefinition.Identifier
                            },
                            Personnel = (q.Personnel == null)
                                ? null
                                : new Option
                                {
                                    Value = q.Personnel.Id,
                                    Label = q.Personnel.Name,
                                },
                            Room = (q.Room == null)
                                ? null
                                : new Option
                                {
                                    Value = q.Room.Id,
                                    Label = q.Room.Identifier,
                                },
                        })).ToList();

                return Json(viewModel) ;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while retrieving allocations", ResponseStatus.Fail);
            }
        }

        // -----------------------------------------------------------------------------
        private async Task ClosePreviousAllocations(string equipmentId)
        {
            if (await _unitOfWork.EquipmentAllocations
                .isExists(q => q.EquipmentID == equipmentId && q.DateReturned == null))
                (await _unitOfWork.EquipmentAllocations
                    .FindAll<EquipmentAllocation>(q => q.EquipmentID == equipmentId))
                    .ToList()
                    .ForEach(q => q.DateReturned = DateTime.Now);

            await _unitOfWork.Save();
        }
    }
}
