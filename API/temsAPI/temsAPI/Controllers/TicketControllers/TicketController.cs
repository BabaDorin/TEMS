using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
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
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            TicketManager ticketManager,
            ILogger<TEMSController> logger,
            IdentityService identityService,
            SystemConfigurationService systemConfigService) : base(unitOfWork, userManager, logger)
        {
            _ticketManager = ticketManager;
            _identityService = identityService;
            _systemConfigService = systemConfigService;
        }

        [HttpGet("ticket/GetTicketsOfEntity/{entityType}/{entityId}/{includingClosed}/{onlyClosed}/{orderBy?}/{skip?}/{take?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching issues")]
        public async Task<IActionResult> GetTicketsOfEntity(
            string entityType, // "any", "user closed", "user created", "user assigned", "equipment", "room", "personnel"  
            string entityId,
            bool includingClosed,
            bool onlyClosed,
            string orderBy = null, // "priority", "recency", "recency closed"
            int skip = 0,
            int take = int.MaxValue)
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

        [HttpGet("ticket/Reopen/{ticketId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while closing the ticket")]
        public async Task<IActionResult> Reopen(string ticketId)
        {
            var ticket = await _ticketManager.GetById(ticketId);
            if (ticket == null)
                return ReturnResponse("Invalid ticket provided", ResponseStatus.Neutral);

            await _ticketManager.ReopenTicket(ticket);
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("ticket/Close/{ticketId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while closing the ticket")]
        public async Task<IActionResult> Close(string ticketId)
        {
            var ticket = await _ticketManager.GetById(ticketId);
            if (ticket == null)
                return ReturnResponse("Invalid ticket provided", ResponseStatus.Neutral);

            await _ticketManager.CloseTicket(ticket);
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("ticket/Changestatus/{ticketId}/{statusId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while updating the status")]
        public async Task<IActionResult> Changestatus(string ticketId, string statusId)
        {
            var ticket = await _ticketManager.GetById(ticketId);
            if (ticket == null)
                return ReturnResponse("Invalid status provided", ResponseStatus.Neutral);

            var status = await _ticketManager.GetTicketStatusByStatusId(statusId);
            if (status == null)
                return ReturnResponse("Invalid status provided", ResponseStatus.Neutral);

            await _ticketManager.ChangeTicketStatus(ticket, status);
            return ReturnResponse("Success!", ResponseStatus.Success);
        }

        [HttpGet("ticket/Archieve/{ticketId}/{newArchivationState?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while removing the specified ticket")]
        public async Task<IActionResult> Archieve(string ticketId, bool? newArchivationState = true)
        {
            var ticket = (await _unitOfWork.Tickets
                    .Find<Ticket>(q => q.Id == ticketId))
                    .FirstOrDefault();

            if (ticket == null)
                return ReturnResponse("Invalid id provided", ResponseStatus.Neutral);

            ticket.IsArchieved = (bool)newArchivationState;
            await _unitOfWork.Save();

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("ticket/GetStatuses")]
        [DefaultExceptionHandler("An error occured when fetching statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _ticketManager.GetTicketStatuses();
            return Ok(statuses);
        }

        [HttpPost("ticket/Create")]
        [DefaultExceptionHandler("An error occurred when creating the ticket")]
        public async Task<IActionResult> Create([FromBody] AddTicketViewModel viewModel)
        {
            // Check if the function has not been disabled by adminstrators for guests
            if (!_identityService.IsAuthenticated() && !_systemConfigService.AppSettings.AllowGuestsToCreateTickets)
                return ReturnResponse("Creating tickets has been disabled for guets.", ResponseStatus.Neutral);

            var result = await _ticketManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("ticket/Remove/{ticketId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the ticket")]
        public async Task<IActionResult> Remove(string ticketId)
        {
            string result = await _ticketManager.Remove(ticketId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("ticket/GetTickets/{equipmentId}/{roomId}/{personnelId}/{includingClosed}/{onlyClosed}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching tickets")]
        public async Task<IActionResult> GetTickets(
            string equipmentId,
            string roomId,
            string personnelId,
            bool includingClosed,
            bool onlyClosed)
        {
            var tickets = await _ticketManager.GetTickets(
                   equipmentId,
                   roomId,
                   personnelId,
                   includingClosed,
                   onlyClosed);

            return Ok(tickets);
        }

        [HttpGet("ticket/ChangePinStatus/{ticketId}/{status}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while pinning the ticket")]
        public async Task<IActionResult> ChangePinStatus(string ticketId, bool status)
        {
            var ticket = await _ticketManager.GetById(ticketId);
            if (ticket == null)
                return ReturnResponse("Invalid ticket id", ResponseStatus.Neutral);

            string result = await _ticketManager.ChangePinStatus(ticket, status);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("ticket/GetPinnedTickets")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the pinned ticket id")]
        public async Task<IActionResult> GetPinnedTickets()
        {
            var ticket = await _ticketManager.GetPinnedTickets();
            return Ok(ticket);
        }
    }
}
