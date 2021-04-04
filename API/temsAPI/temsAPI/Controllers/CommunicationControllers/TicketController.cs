﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels;
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

        [HttpGet("/ticket/getticketsofentity/{entityType}/{entityId}/{includingClosed}/{onlyClosed}/{orderBy?}/{skip?}/{take?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetTicketsOfEntity(
            string entityType, // "user closed" "user created", "user assigned", "personnel", "equipment", "room"
            string entityId,
            bool includingClosed,
            bool onlyClosed,
            string orderBy = null, // "priority", "recency", "recency closed"
            int? skip = null,
            int? take = null)
        {
            try
            {
                // Invalid IdentityType
                if ((new List<string> { "any", "user closed", "user created", "user assigned", "equipment", "room", "personnel" }).IndexOf(entityType) == -1)
                    return ReturnResponse("Invalid type or id provided", ResponseStatus.Fail);

                // No identityId Provided
                if (String.IsNullOrEmpty(entityId.Trim()))
                    return ReturnResponse($"You have to provide a valid {entityType} Id", ResponseStatus.Fail);

                // Checking if identityId is valid and at the same time we build the expression

                // false false
                Expression<Func<Ticket, bool>> expression = q => q.DateClosed == null && !q.IsArchieved;

                // true false
                if (includingClosed)
                    expression = q => !q.IsArchieved;

                // any true
                if (onlyClosed)
                    expression = q => q.DateClosed.HasValue && !q.IsArchieved;
                
                Expression<Func<Ticket, bool>> expression2 = null;

                switch (entityType)
                {
                    case "user closed":
                        if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == entityId))
                            return ReturnResponse("Invalid type or id provided", ResponseStatus.Fail);

                        expression2 = q => q.ClosedById == entityId;
                        break;
                    case "user created":
                        if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == entityId))
                            return ReturnResponse("Invalid type or id provided", ResponseStatus.Fail);

                        expression2 = q => q.CreatedById == entityId;
                        break;
                    case "user assigned":
                        var user = await _userManager.FindByIdAsync(entityId);
                        if (user == null)
                            return ReturnResponse("Invalid type or id provided", ResponseStatus.Fail);

                        expression2 = q => q.Assignees.Contains(user);
                        break;
                    case "equipment":
                        if (!await _unitOfWork.Equipments.isExists(q => q.Id == entityId))
                            return ReturnResponse("Invalid type or id provided", ResponseStatus.Fail);

                        expression2 = q => q.Equipments.Any(q => q.Id == entityId);
                        break;
                    case "room":
                        if (!await _unitOfWork.Rooms.isExists(q => q.Id == entityId))
                            return ReturnResponse("Invalid type or id provided", ResponseStatus.Fail);

                        expression2 = q => q.Rooms.Any(q => q.Id == entityId);
                        break;
                    case "personnel":
                        if (!await _unitOfWork.Personnel.isExists(q => q.Id == entityId))
                            return ReturnResponse("Invalid type or id provided", ResponseStatus.Fail);

                        expression2 = q => q.Personnel.Any(q => q.Id == entityId);
                        break;
                }

                expression = ExpressionCombiner.CombineTwo(expression, expression2);

                Func<IQueryable<Ticket>, IOrderedQueryable<Ticket>> orderByExpression = null;
                switch (orderBy)
                {
                    case "recency":
                        orderByExpression = q => q.OrderByDescending(q => q.Status.ImportanceIndex);
                        break;
                    case "recency closed":
                        orderByExpression = q => q.OrderByDescending(q => q.DateClosed);
                        break;
                    default:
                        orderByExpression = q => q.OrderByDescending(q => q.DateCreated);
                        break;
                }

                List<ViewTicketSimplifiedViewModel> viewModel = (await _unitOfWork.Tickets
                    .FindAll<ViewTicketSimplifiedViewModel>(
                        include: q => q.Include(q => q.Assignees)
                                       .Include(q => q.ClosedBy)
                                       .Include(q => q.CreatedBy)
                                       .Include(q => q.Label)
                                       .Include(q => q.Personnel)
                                       .Include(q => q.Rooms)
                                       .Include(q => q.Status)
                                       .Include(q => q.Equipments),
                        where: expression,
                        orderBy: orderByExpression,
                        skip: skip ?? 0,
                        take: take ?? int.MaxValue,
                        select: q => new ViewTicketSimplifiedViewModel
                        {
                            Id = q.Id,
                            Problem = q.Problem,
                            DateClosed = q.DateClosed,
                            DateCreated = q.DateCreated,
                            Description = q.Description,
                            Equipments = q.Equipments.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.TemsIdOrSerialNumber
                            }).ToList(),
                            Label = new Option
                            {
                                Value = q.Label.Id,
                                Label = q.Label.Name,
                                Additional = q.Label.ColorHex
                            },
                            Personnel = q.Personnel.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.Name
                            }).ToList(),
                            Rooms = q.Rooms.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.Identifier,
                                Additional = string.Join(", ", q.Labels)
                            }).ToList(),
                            Assignees = q.Assignees.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.FullName ?? q.UserName,
                            }).ToList(),
                            Status = new Option
                            {
                                Value = q.Status.Id,
                                Label = q.Status.Name,
                                Additional = q.Status.ImportanceIndex.ToString()
                            },
                            ClosedBy = (q.ClosedById == null)
                            ? null
                            : new Option
                            {
                                Label = q.ClosedBy.FullName ?? q.ClosedBy.UserName,
                                Value = q.ClosedById
                            }
                        }
                    )).ToList();

                return (Json(viewModel));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching issues", ResponseStatus.Fail);
            }
        }

        [HttpGet("/ticket/reopen/{ticketId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Reopen(string ticketId)
        {
            try
            {
                var ticket = (await _unitOfWork.Tickets
                    .Find<Ticket>(q => q.Id == ticketId))
                    .FirstOrDefault();

                if (ticket == null)
                    return ReturnResponse("Invalid ticket provided", ResponseStatus.Fail);

                ticket.ClosedById = IdentityHelper.GetUserId(User);
                ticket.DateClosed = null;

                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while closing the ticket", ResponseStatus.Fail);
            }
        }

        [HttpGet("/ticket/close/{ticketId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Close(string ticketId)
        {
            try
            {
                var ticket = (await _unitOfWork.Tickets
                    .Find<Ticket>(q => q.Id == ticketId))
                    .FirstOrDefault();

                if (ticket == null)
                    return ReturnResponse("Invalid ticket provided", ResponseStatus.Fail);

                ticket.ClosedById = IdentityHelper.GetUserId(User);
                ticket.DateClosed = DateTime.Now;

                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while closing the ticket", ResponseStatus.Fail);
            }
        }

        [HttpGet("/ticket/changestatus/{ticketId}/{statusId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Remove(string ticketId, string statusId)
        {
            try
            {
                var ticket = (await _unitOfWork.Tickets
                    .Find<Ticket>(q => q.Id == ticketId))
                    .FirstOrDefault();

                if (ticket == null)
                    return ReturnResponse("Invalid status provided", ResponseStatus.Fail);

                var newStatus = (await _unitOfWork.Statuses
                    .Find<Status>(q => q.Id == statusId))
                    .FirstOrDefault();

                if (newStatus == null)
                    return ReturnResponse("Invalid status provided", ResponseStatus.Fail);

                ticket.Status = newStatus;
                await _unitOfWork.Save();

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while updating the status", ResponseStatus.Fail);
            }
        }


        [HttpGet("/ticket/archieve/{ticketId}/{newArchivationState?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string ticketId, bool? newArchivationState = true)
        {
            try
            {
                var ticket = (await _unitOfWork.Tickets
                    .Find<Ticket>(q => q.Id == ticketId))
                    .FirstOrDefault();

                if (ticket == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                ticket.IsArchieved = (bool)newArchivationState;
                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while removing the specified ticket", ResponseStatus.Fail);
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
                    CreatedBy = (IdentityHelper.isAuthenticated(User)) 
                        ? await _userManager.FindByIdAsync(IdentityHelper.GetUserId(User)) 
                        : null,
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

        [HttpGet("/ticket/gettickets/{equipmentId}/{roomId}/{personnelId}/{includingClosed}/{onlyClosed}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetTickets(
            string equipmentId,
            string roomId,
            string personnelId,
            bool includingClosed,
            bool onlyClosed)
        {                                                           // Announcements
            try
            {
                // Invalid equipmentId
                if (equipmentId != "any" && !await _unitOfWork.Equipments.isExists(q => q.Id == equipmentId))
                    return ReturnResponse("Invalid equipment provided", ResponseStatus.Fail);

                // Invalid roomId provided
                if (roomId != "any" && !await _unitOfWork.Rooms.isExists(q => q.Id == roomId))
                    return ReturnResponse("Invalid room provided", ResponseStatus.Fail);

                // Invalid personnelId provided
                if (personnelId != "any" && !await _unitOfWork.Personnel.isExists(q => q.Id == personnelId))
                    return ReturnResponse("Invalid personnel provied", ResponseStatus.Fail);

                // false false
                Expression<Func<Ticket, bool>> closedExpression = q => q.DateClosed == null && !q.IsArchieved;

                // true false
                if (includingClosed)
                    closedExpression = q => !q.IsArchieved;

                // any true
                if (onlyClosed)
                    closedExpression = q => q.DateClosed.HasValue && !q.IsArchieved;

                Expression<Func<Ticket, bool>> equipmentExpression = (equipmentId == "any")
                   ? null
                   : q => q.Equipments.Any(q => q.Id == equipmentId);

                Expression<Func<Ticket, bool>> roomExpression = (roomId == "any")
                   ? null
                   : q => q.Rooms.Any(q => q.Id == roomId);

                Expression<Func<Ticket, bool>> personnelExpression = (personnelId == "any")
                   ? null
                   : q => q.Personnel.Any(q => q.Id == personnelId);

                var finalExpression = ExpressionCombiner.And(closedExpression, equipmentExpression, roomExpression, personnelExpression);

                List<ViewTicketSimplifiedViewModel> viewModel = (await _unitOfWork.Tickets
                    .FindAll<ViewTicketSimplifiedViewModel>(
                        where: finalExpression,
                        include: q => q.Include(q => q.Assignees)
                                       .Include(q => q.ClosedBy)
                                       .Include(q => q.CreatedBy)
                                       .Include(q => q.Label)
                                       .Include(q => q.Personnel)
                                       .Include(q => q.Rooms)
                                       .Include(q => q.Status)
                                       .Include(q => q.Equipments)
                                       .Include(q => q.Assignees),
                        orderBy: q => q.OrderBy(q => q.Status.ImportanceIndex)
                                       .ThenByDescending(q => q.DateCreated),
                        select: q => new ViewTicketSimplifiedViewModel
                        {
                            Id = q.Id,
                            Problem = q.Problem,
                            DateClosed = q.DateClosed,
                            DateCreated = q.DateCreated,
                            Description = q.Description,
                            Equipments = q.Equipments.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.TemsIdOrSerialNumber
                            }).ToList(),
                            Label = new Option
                            {
                                Value = q.Label.Id,
                                Label = q.Label.Name,
                                Additional = q.Label.ColorHex
                            },
                            Personnel = q.Personnel.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.Name
                            }).ToList(),
                            Rooms = q.Rooms.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.Identifier,
                                Additional = string.Join(", ", q.Labels)
                            }).ToList(),
                            Assignees = q.Assignees.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.FullName ?? q.UserName,
                            }).ToList(),
                            Status = new Option
                            {
                                Value = q.Status.Id,
                                Label = q.Status.Name,
                                Additional = q.Status.ImportanceIndex.ToString()
                            },
                            ClosedBy = (q.ClosedById == null)
                            ? null
                            : new Option
                            {
                                Label = q.ClosedBy.FullName ?? q.ClosedBy.UserName,
                                Value = q.ClosedById
                            }
                        }
                    )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching tickets", ResponseStatus.Fail);
            }
        }
    }
}
