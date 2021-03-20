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
using temsAPI.ViewModels.Personnel;
using temsAPI.ViewModels.Room;

namespace temsAPI.Controllers.RoomControllers
{
    public class RoomController : TEMSController
    {
        public RoomController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager
            ) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet]
        public async Task<JsonResult> GetAllAutocompleteOptions()
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork.Rooms.FindAll<Option>(
                    where: q => !q.IsArchieved,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Identifier,
                        Additional = q.Description
                    })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddRoomViewModel viewModel)
        {
            try
            {
                // Indentifier is not valid
                if (String.IsNullOrEmpty(viewModel.Identifier = viewModel.Identifier.Trim()))
                    return ReturnResponse("Please, provide a valid room identifier", ResponseStatus.Fail);

                // This room already exists
                if (await _unitOfWork.Rooms.isExists(q => q.Identifier == viewModel.Identifier && !q.IsArchieved))
                    return ReturnResponse($"This {viewModel.Identifier} room already exists", ResponseStatus.Fail);

                // Invalid labels provided
                foreach (Option label in viewModel.Labels)
                    if (!await _unitOfWork.RoomLabels.isExists(q => q.Id == label.Value && !q.IsArchieved))
                        return ReturnResponse("Invalid labels provided", ResponseStatus.Fail);

                // Might be valid
                List<string> labelIds = viewModel.Labels.Select(q => q.Value).ToList();
                Room model = new Room
                {
                    Id = Guid.NewGuid().ToString(),
                    Description = viewModel.Description,
                    Floor = viewModel.Floor,
                    Identifier = viewModel.Identifier,
                    Labels = await _unitOfWork.RoomLabels.FindAll<RoomLabel>(
                        where: q => labelIds.Contains(q.Id))
                };

                await _unitOfWork.Rooms.Create(model);
                await _unitOfWork.Save();

                if (!await _unitOfWork.Rooms.isExists(q => q.Id == model.Id))
                    return ReturnResponse("Fail", ResponseStatus.Fail);
                else
                    return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the room", ResponseStatus.Fail);
            }
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
                                   .Include(q => q.EquipmentAllocations)
                                   .Include(q => q.Tickets.Where(q => q.DateClosed == null)),
                    select: q => new ViewRoomSimplifiedViewModel
                    {
                        Id = q.Id,
                        Description = q.Description,
                        AllocatedEquipments = q.EquipmentAllocations.Count(q => q.RoomID != null && q.DateReturned == null),
                        Identifier = q.Identifier,
                        Label = string.Join(", ", q.Labels.Select(q => q.Name)),
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

        [HttpGet("/room/getbyid/{id}")]
        public async Task<JsonResult> GetById(string id)
        {
            try
            {
                // Invalid id provided
                if (String.IsNullOrEmpty(id) || !await _unitOfWork.Rooms.isExists(q => q.Id == id))
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                var viewModel = (await _unitOfWork.Rooms
                    .Find<ViewRoomViewModel>(
                        where: q => q.Id == id,
                        include: q => q.Include(q => q.Labels)
                                       .Include(q => q.PersonnelRoomSupervisories)
                                            .ThenInclude(q => q.Personnel)
                                            .ThenInclude(q => q.Positions)
                                       .Include(q => q.Tickets),
                        select: q => new ViewRoomViewModel
                        {
                            Id = q.Id,
                            Description = q.Description,
                            Identifier = q.Identifier,
                            Floor = q.Floor ?? 0,
                            ActiveTickets = q.Tickets.Count(q => q.DateClosed == null),
                            Labels = q.Labels.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.Name
                            }).ToList(),
                            Supervisory = q.PersonnelRoomSupervisories.Select(q => new ViewPersonnelSimplifiedViewModel
                            {
                                Id = q.Id,
                                Name = q.Personnel.Name,
                                Positions = string.Join(", ", q.Personnel.Positions)
                            }).ToList(),
                        }
                    )).FirstOrDefault();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching the room", ResponseStatus.Fail);
            }
        }
    }
}
