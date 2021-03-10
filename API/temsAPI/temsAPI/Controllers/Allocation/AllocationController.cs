using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Any;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
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
                if ((new List<string> { "personnel", "room" }).IndexOf(viewModel.AllocatedToType) == -1)
                    return ReturnResponse("Invalid type provided", ResponseStatus.Fail);

                // No allocation id provided or the provided one is invalid
                if (String.IsNullOrEmpty(viewModel.AllocatedToType))
                    return ReturnResponse("Please, provide a valid allocation object type", ResponseStatus.Fail);

                List<string> equipmentsWhereFailed = new List<string>();
                if (viewModel.AllocatedToType == "personnel")
                {
                    if (!await _unitOfWork.Personnel.isExists(q => q.Id == viewModel.AllocatedToId))
                        return ReturnResponse("Allocatee id seems invalid.", ResponseStatus.Fail);

                    foreach (Option equipment in viewModel.Equipments)
                    {
                        var model = new PersonnelEquipmentAllocation
                        {
                            Id = Guid.NewGuid().ToString(),
                            DateAllocated = DateTime.Now,
                            EquipmentID = equipment.Value,
                            PersonnelID = viewModel.AllocatedToId
                        };

                        await _unitOfWork.PersonnelEquipmentAllocations.Create(model);
                        await _unitOfWork.Save();

                        if (!await _unitOfWork.PersonnelEquipmentAllocations.isExists(q => q.Id == model.Id))
                            equipmentsWhereFailed.Add(equipment.Label);
                        else
                            ClosePreviousAllocations(model.EquipmentID);
                    }
                }


                if (viewModel.AllocatedToType == "room")
                {
                    if (!await _unitOfWork.Rooms.isExists(q => q.Id == viewModel.AllocatedToId))
                        return ReturnResponse("Allocatee id seems invalid.", ResponseStatus.Fail);

                    foreach (Option equipment in viewModel.Equipments)
                    {
                        var model = new RoomEquipmentAllocation
                        {
                            Id = Guid.NewGuid().ToString(),
                            DateAllocated = DateTime.Now,
                            EquipmentID = equipment.Value,
                            RoomID = viewModel.AllocatedToId
                        };

                        await _unitOfWork.RoomEquipmentAllocations.Create(model);
                        await _unitOfWork.Save();

                        if (!await _unitOfWork.RoomEquipmentAllocations.isExists(q => q.Id == model.Id))
                            equipmentsWhereFailed.Add(equipment.Label);
                        else
                            ClosePreviousAllocations(model.EquipmentID);
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

        // :(
        public async void ClosePreviousAllocations(string equipmentId)
        {
            if(await _unitOfWork.RoomEquipmentAllocations
                .isExists(q => q.EquipmentID == equipmentId && q.DateReturned == null))
                    (await _unitOfWork.RoomEquipmentAllocations
                        .FindAll<RoomEquipmentAllocation>(q => q.EquipmentID == equipmentId))
                        .ToList()
                        .ForEach(q => q.DateReturned = DateTime.Now);

            if (await _unitOfWork.PersonnelEquipmentAllocations
                .isExists(q => q.EquipmentID == equipmentId && q.DateReturned == null))
                (await _unitOfWork.PersonnelEquipmentAllocations
                    .FindAll<PersonnelEquipmentAllocation>(q => q.EquipmentID == equipmentId))
                    .ToList()
                    .ForEach(q => q.DateReturned = DateTime.Now);
        }
    }
}
