using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Room;

namespace temsAPI.Controllers.RoomControllers
{
    public class RoomController : TEMSController
    {
        public RoomController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet("/room/getsimplified/{pageNumber}/{recordsPerPage}")]
        public async Task<JsonResult> GetSimplified(int pageNumber, int recordsPerPage)
        {
            try
            {
                // Invalid page number or records per page provided
                if (pageNumber < 0 || recordsPerPage < 0 || pageNumber > 1000 || recordsPerPage > 1000)
                    return ReturnResponse("Invalid parameters, please provide real number representing page number" +
                        "and how many records are displayed per page", ResponseStatus.Fail);

                List<ViewRoomSimplifiedViewModel> viewModel = (await _unitOfWork.Rooms.FindAll<ViewRoomSimplifiedViewModel>(
                    where: q => !q.IsArchieved,
                    include: q => q.Include(q => q.Labels)
                                   .Include(q => q.RoomEquipmentAllocations)
                                   .Include(q => q.Tickets),
                    select: q => new ViewRoomSimplifiedViewModel
                    {
                        Id = q.Id,
                        Description = q.Description,
                        AllocatedEquipments = q.RoomEquipmentAllocations.Count,
                        Identifier = q.Identifier,
                        Label = string.Join(", ", q.Labels),
                        ActiveTickets = q.Tickets.Count
                    })).OrderBy(q => q.Identifier).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching rooms", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetLabels()
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork.RoomLabels.FindAll<Option>(
                    where: q => !q.IsArchieved,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching room labels", ResponseStatus.Fail);
            }
        }
    }
}
