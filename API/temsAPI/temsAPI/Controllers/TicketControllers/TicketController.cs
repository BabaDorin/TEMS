﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels.Ticket;

namespace temsAPI.Controllers.TicketControllers
{
    // BEFREE: Use only one endpoint for retrieving tickets.
    // Group filters within a viewmodel

    public class TicketController : TEMSController
    {
        private TicketManager _ticketManager;
        private IdentityService _identityService;
        private SystemConfigurationService _systemConfigService;

        public TicketController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            TicketManager ticketManager,
            ILogger<TEMSController> logger,
            IdentityService identityService,
            SystemConfigurationService systemConfigService) : base(mapper, unitOfWork, userManager, logger)
        {
            _ticketManager = ticketManager;
            _identityService = identityService;
            _systemConfigService = systemConfigService;
        }

        [HttpGet("ticket/GetTicketsOfEntity/{entityType}/{entityId}/{includingClosed}/{onlyClosed}/{orderBy?}/{skip?}/{take?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetTicketsOfEntity(
            string entityType, // "any", "user closed", "user created", "user assigned", "equipment", "room", "personnel"  
            string entityId,
            bool includingClosed,
            bool onlyClosed,
            string orderBy = null, // "priority", "recency", "recency closed"
            int skip = 0,
            int take = int.MaxValue)
        {
            try
            {
                var tickets = await _ticketManager.GetEntityTickets(
                    entityType,
                    entityId,
                    includingClosed,
                    onlyClosed,
                    orderBy,
                    skip, take);

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching issues", ResponseStatus.Fail);
            }
        }

        [HttpGet("ticket/Reopen/{ticketId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Reopen(string ticketId)
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
                LogException(ex);
                return ReturnResponse("An error occured while closing the ticket", ResponseStatus.Fail);
            }
        }

        [HttpGet("ticket/Close/{ticketId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Close(string ticketId)
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
                LogException(ex);
                return ReturnResponse("An error occured while closing the ticket", ResponseStatus.Fail);
            }
        }

        [HttpGet("ticket/Changestatus/{ticketId}/{statusId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Changestatus(string ticketId, string statusId)
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
                LogException(ex);
                return ReturnResponse("An error occured while updating the status", ResponseStatus.Fail);
            }
        }

        [HttpGet("ticket/Archieve/{ticketId}/{newArchivationState?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Archieve(string ticketId, bool? newArchivationState = true)
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
                LogException(ex);
                return ReturnResponse("An error occured while removing the specified ticket", ResponseStatus.Fail);
            }
        }

        [HttpGet("ticket/GetStatuses")]
        public async Task<IActionResult> GetStatuses()
        {
            try
            {
                var statuses = await _ticketManager.GetTicketStatuses();
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching statuses", ResponseStatus.Fail);
            }
        }

        [HttpPost("ticket/Create")]
        public async Task<IActionResult> Create([FromBody] AddTicketViewModel viewModel)
        {
            try
            {
                // Check if the function has not been disabled by adminstrators for guests
                if (!_identityService.IsAuthenticated() && !_systemConfigService.AppSettings.AllowGuestsToCreateTickets)
                    return ReturnResponse("Creating tickets has been disabled for guets.", ResponseStatus.Fail);

                var result = await _ticketManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occurred when creating the ticket", ResponseStatus.Fail);
            }
        }

        [HttpDelete("ticket/Remove/{ticketId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<IActionResult> Remove(string ticketId)
        {
            try
            {
                string result = await _ticketManager.Remove(ticketId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the ticket", ResponseStatus.Fail);
            }
        }

        [HttpGet("ticket/GetTickets/{equipmentId}/{roomId}/{personnelId}/{includingClosed}/{onlyClosed}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetTickets(
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

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching tickets", ResponseStatus.Fail);
            }
        }

        [HttpGet("ticket/ChangePinStatus/{ticketId}/{status}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> ChangePinStatus(string ticketId, bool status)
        {
            try
            {
                var ticket = await _ticketManager.GetById(ticketId);
                if (ticket == null)
                    return ReturnResponse("Invalid ticket id", ResponseStatus.Fail);

                string result = await _ticketManager.ChangePinStatus(ticket, status);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while pinning the ticket", ResponseStatus.Fail);
            }
        }

        [HttpGet("ticket/GetPinnedTickets")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetPinnedTickets()
        {
            try
            {
                var ticket = await _ticketManager.GetPinnedTickets();
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching the pinned ticket id", ResponseStatus.Fail);
            }
        }
    }
}
