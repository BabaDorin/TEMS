using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Status;
using temsAPI.ViewModels.Ticket;

namespace temsAPI.Controllers.CommunicationControllers
{
    public class TicketController : TEMSController
    {
        public TicketController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet("/ticket/{identityType}/{identityId}/{includingClosed}/{onlyClosed}")]
        public async Task<JsonResult> Get(
            string identityType,
            string identityId,
            bool includingClosed,
            bool onlyClosed)
        {
            try
            {
                // Invalid IdentityType
                if ((new List<string> { "equipment", "room", "personnel" }).IndexOf(identityType) == -1)
                    return ReturnResponse("Invalid identity type.", ResponseStatus.Fail);

                // No identityId Provided
                if (String.IsNullOrEmpty(identityId.Trim()))
                    return ReturnResponse($"You have to provide a valid {identityType} Id", ResponseStatus.Fail);

                // Checking if identityId is valid and at the same time we build the expression

                // false false
                Expression<Func<Ticket, bool>> ticketExpression = qu => qu.DateClosed == null;

                // true false
                if (includingClosed)
                    ticketExpression = null;

                if (onlyClosed)
                    ticketExpression = qu => qu.DateClosed.HasValue;

                // The tickets that are going to be extracted
                List<Ticket> tickets = new List<Ticket>();

                switch (identityType)
                {
                    case "equipment":
                        if (!await _unitOfWork.Equipments.isExists(q => q.Id == identityId))
                            return ReturnResponse($"There is no {identityType} having the specified Id", ResponseStatus.Fail);

                        tickets = (await _unitOfWork.Equipments.Find<Equipment>(
                            where: q => q.Id == identityId,
                            include: q => q
                                .Include(q => q.Tickets).ThenInclude(q => q.Status)
                                .Include(q => q.Tickets).ThenInclude(q => q.Label)))
                            .FirstOrDefault()
                            .Tickets.ToList();

                        break;

                    case "room":
                        if (!await _unitOfWork.Rooms.isExists(q => q.Id == identityId))
                            return ReturnResponse($"There is no {identityType} having the specified Id", ResponseStatus.Fail);

                        tickets = (await _unitOfWork.Rooms.Find<Room>(
                             where: q => q.Id == identityId,
                             include: q => q.Include(q => q.Tickets).ThenInclude(q => q.Equipments)))
                             .FirstOrDefault()
                             .Tickets.ToList();
                        break;

                    case "personnel":
                        if (!await _unitOfWork.Personnel.isExists(q => q.Id == identityId))
                            return ReturnResponse($"There is no {identityType} having the specified Id", ResponseStatus.Fail);

                        tickets = (await _unitOfWork.Personnel.Find<Personnel>(
                             where: q => q.Id == identityId,
                             include: q => q.Include(q => q.Tickets).ThenInclude(q => q.Equipments)))
                             .FirstOrDefault()
                             .Tickets.ToList();
                        break;
                }

                List<ViewTicketSimplifiedViewModel> viewModel =
                           tickets.AsQueryable().Where(ticketExpression)
                           .ToList()
                           .Select(q => new ViewTicketSimplifiedViewModel
                           {
                               Id = q.Id,
                               DateClosed = q.DateClosed,
                               DateCreated = q.DateCreated,
                               Description = q.Description,
                               Problem = q.Problem,
                               Status = q.Status.Name,
                               Label = (q.Label != null) 
                                    ? new Option { 
                                           Label = q.Label.Name, 
                                           Additional = q.Label.ColorHex
                                      }
                                    : null,
                               Equipments = q.Equipments
                                               .Select(q => new Option
                                               {
                                                   Label = q.TemsIdOrSerialNumber,
                                                   Value = q.Id
                                               })?.ToList(),
                               Personnel = q.Personnel
                                               .Select(q => new Option
                                               {
                                                   Label = q.Name,
                                                   Value = q.Id
                                               })?.ToList(),
                               Rooms = q.Rooms
                                           .Select(q => new Option
                                           {
                                               Label = q.Identifier,
                                               Value = q.Id,
                                           })?.ToList()
                           }).ToList();

                return (Json(viewModel));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching issues", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetStatuses()
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork
                    .Statuses
                    .FindAll<Option>(
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
                return ReturnResponse("An error occured when fetching statuses", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddTicketViewModel viewModel)
        {
            try
            {
                // Problem has not been defined
                if (String.IsNullOrEmpty(viewModel.Problem = viewModel.Problem.Trim()))
                    return ReturnResponse("Please, indicate the ticket's problem.", ResponseStatus.Fail);

                // No status or invalid
                if (String.IsNullOrEmpty(viewModel.Status = viewModel.Status.Trim()) ||
                    !await _unitOfWork.Statuses.isExists(q => q.Id == viewModel.Status))
                    return ReturnResponse("Invalid status provided.", ResponseStatus.Fail);

                // Validating participants (equipments, rooms, personnel)
                if (viewModel.Equipments.Count > 0)
                    foreach (Option op in viewModel.Equipments)
                        if (!await _unitOfWork.Equipments.isExists(q => q.Id == op.Value))
                            return ReturnResponse("One or more Equipments are invalid", ResponseStatus.Fail);

                if (viewModel.Personnel.Count > 0)
                    foreach (Option op in viewModel.Personnel)
                        if (!await _unitOfWork.Personnel.isExists(q => q.Id == op.Value))
                            return ReturnResponse("One or more Personnel are invalid", ResponseStatus.Fail);

                if (viewModel.Rooms.Count > 0)
                    foreach (Option op in viewModel.Rooms)
                        if (!await _unitOfWork.Rooms.isExists(q => q.Id == op.Value))
                            return ReturnResponse("One or more Rooms are invalid", ResponseStatus.Fail);

                if (viewModel.Assignees.Count > 0)
                    foreach (Option op in viewModel.Assignees)
                        if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == op.Value))
                            return ReturnResponse("One or more Assignees are invalid", ResponseStatus.Fail);

                // Seems valid
                Ticket model = new Ticket
                {
                    Id = Guid.NewGuid().ToString(),
                    StatusId = viewModel.Status,
                    Problem = viewModel.Problem,
                    DateCreated = DateTime.Now,
                    Description = viewModel.ProblemDescription,
                    Rooms = await _unitOfWork.Rooms.FindAll<Room>(
                        where: q =>
                            viewModel.Rooms.Select(q => q.Value).Contains(q.Id)),
                    Equipments = await _unitOfWork.Equipments.FindAll<Equipment>(
                        where: q =>
                            viewModel.Equipments.Select(q => q.Value).Contains(q.Id)),
                    Personnel = await _unitOfWork.Personnel.FindAll<Personnel>(
                        where: q =>
                            viewModel.Personnel.Select(q => q.Value).Contains(q.Id)),
                    Assignees = await _unitOfWork.TEMSUsers.FindAll<TEMSUser>(
                        where: q =>
                            viewModel.Assignees.Select(q => q.Value).Contains(q.Id)),
                };

                await _unitOfWork.Tickets.Create(model);
                await _unitOfWork.Save();

                if (await _unitOfWork.Tickets.isExists(q => q.Id == model.Id))
                    return ReturnResponse("Success!", ResponseStatus.Success);
                else
                    return ReturnResponse("Fail", ResponseStatus.Fail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occurred when creating the ticket", ResponseStatus.Fail);
            }
        }
    }
}
