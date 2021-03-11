using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public async Task ClosePreviousAllocations(string equipmentId)
        {
            if(await _unitOfWork.EquipmentAllocations
                .isExists(q => q.EquipmentID == equipmentId && q.DateReturned == null))
                    (await _unitOfWork.EquipmentAllocations
                        .FindAll<EquipmentAllocation>(q => q.EquipmentID == equipmentId))
                        .ToList()
                        .ForEach(q => q.DateReturned = DateTime.Now);

            await _unitOfWork.Save();
        }

        //[HttpGet("allocation/getofentity/{entityType}/{entityId}")]
        //public async Task<JsonResult> GetOfEntity(string entityType, string entityId)
        //{
        //    try
        //    {
        //        // Invalid identityType
        //        if ((new List<string>() { "any", "equipment", "room", "personnel" }).IndexOf(entityType) == -1)
        //            return ReturnResponse("Invalid entitytype provided", ResponseStatus.Fail);

        //        // No entity id provided
        //        if (String.IsNullOrEmpty(entityId.Trim()))
        //            return ReturnResponse($"You have to provide a valid {entityType} Id", ResponseStatus.Fail);

        //        Expression<Func<Allocation, bool>> expression = q => q.DateClosed == null && !q.IsArchieved;

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        return ReturnResponse("An error occured when fetching entity's allocations", ResponseStatus.Fail);
        //    }
        //}
    }
}
