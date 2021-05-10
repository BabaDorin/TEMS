using AutoMapper;
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
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Ticket;

namespace temsAPI.Controllers.TicketControllers
{
    public class TicketController : TEMSController
    {
        private TicketManager _ticketManager;
        public TicketController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            TicketManager ticketManager) : base(mapper, unitOfWork, userManager)
        {
            _ticketManager = ticketManager;
        }

        [HttpGet("/ticket/getticketsofentity/{entityType}/{entityId}/{includingClosed}/{onlyClosed}/{orderBy?}/{skip?}/{take?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetTicketsOfEntity(
            string entityType, // "any", "user closed", "user created", "user assigned", "equipment", "room", "personnel"  
            string entityId,
            bool includingClosed,
            bool onlyClosed,
            string orderBy = null, // "priority", "recency", "recency closed"
            int? skip = null,
            int? take = null)
        {
            try
            {
                var tickets = await _ticketManager.GetEntityTickets(
                    entityType,
                    entityId,
                    includingClosed,
                    onlyClosed,
                    orderBy);

                return Json(tickets);
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
                var ticket = await _ticketManager.GetById(ticketId);
                if (ticket == null)
                    return ReturnResponse("Invalid ticket provided", ResponseStatus.Fail);

                await _ticketManager.ReopenTicket(ticket);
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
                var ticket = await _ticketManager.GetById(ticketId);
                if (ticket == null)
                    return ReturnResponse("Invalid ticket provided", ResponseStatus.Fail);

                await _ticketManager.CloseTicket(ticket);
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
        public async Task<JsonResult> Changestatus(string ticketId, string statusId)
        {
            try
            {
                var ticket = await _ticketManager.GetById(ticketId);
                if (ticket == null)
                    return ReturnResponse("Invalid status provided", ResponseStatus.Fail);

                var status = await _ticketManager.GetTicketStatusByStatusId(statusId);
                if (status == null)
                    return ReturnResponse("Invalid status provided", ResponseStatus.Fail);

                await _ticketManager.ChangeTicketStatus(ticket, status);
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
                var statuses = await _ticketManager.GetTicketStatuses();
                return Json(statuses);
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
                var result = await _ticketManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
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
        {
            try
            {
                var tickets = await _ticketManager.GetTickets(
                    equipmentId,
                    roomId,
                    personnelId,
                    includingClosed,
                    onlyClosed);

                return Json(tickets);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching tickets", ResponseStatus.Fail);
            }
        }
    }
}
