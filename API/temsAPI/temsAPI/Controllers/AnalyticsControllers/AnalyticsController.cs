using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers.AnalyticsHelpers.AnalyticsModels;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;

namespace temsAPI.Controllers.AnalyticsControllers
{
    public class AnalyticsController : TEMSController
    {
        private AnalyticsManager _analyticsManager;

        public AnalyticsController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            AnalyticsManager analyticsManager,
            CurrencyConvertor currencyConvertor,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _analyticsManager = analyticsManager;

        }

        [HttpGet("analytics/GetEquipmentAmount/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while total equipment amount")]
        public async Task<IActionResult> GetEquipmentAmount(
            string entityType = null,
            string entityId = null)
        {
            var totalNumberOfEquipment = await _analyticsManager.GetEquipmentAmount(entityType, entityId);
            return Ok(totalNumberOfEquipment);
        }

        [HttpGet("analytics/GetEquipmentTotalCost/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while calculating total equipment cost")]
        public async Task<IActionResult> GetEquipmentTotalCost(
            string entityType = null,
            string entityId = null)
        {
            double totalCost = await _analyticsManager.GetEquipmentTotalCost(entityType, entityId);
            return Ok(Math.Round(totalCost, 2));
        }

        [HttpGet("analytics/GetEquipmentUtilizationRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while calculating equipment utilization rates")]
        public async Task<IActionResult> GetEquipmentUtilizationRate(
            string entityType = null,
            string entityId = null)
        {
            var pieChart = await _analyticsManager.GetEquipmentUtilizationRate(entityType, entityId);
            return Ok(pieChart);
        }

        [HttpGet("analytics/GetEquipmentTypeRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while calculating equipment type rates")]
        public async Task<IActionResult> GetEquipmentTypeRate(
            string entityType = null,
            string entityId = null)
        {
            var pieChart = await _analyticsManager.GetEquipmentTypeRate(entityType, entityId);
            return Ok(pieChart);
        }

        [HttpGet("analytics/GetEquipmentAllocationRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while calculating equipment allocation rates")]
        public async Task<IActionResult> GetEquipmentAllocationRate(
            string entityType = null,
            string entityId = null)
        {
            var pieChart = await _analyticsManager.GetEquipmentAllocationRate(entityType, entityId);
            return Ok(pieChart);
        }

        [HttpGet("analytics/GetEquipmentWorkabilityRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while calculating equipment workability rates")]
        public async Task<IActionResult> GetEquipmentWorkabilityRate(
            string entityType = null,
            string entityId = null)
        {
            var pieChart = await _analyticsManager.GetEquipmentWorkabilityRate(entityType, entityId);
            return Ok(pieChart);
        }

        [HttpGet("analytics/GetTicketClosingRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while calculating ticket closing rates")]
        public async Task<IActionResult> GetTicketClosingRate(
            string entityType,
            string entityId)
        {
            var pieChart = await _analyticsManager.GetTicketClosingRate(entityType, entityId);
            return Ok(pieChart);
        }

        [HttpGet("analytics/GetTicketClosingByRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while calculating ticket closing by rates")]
        public async Task<IActionResult> GetTicketClosingByRate(
            string entityType,
            string entityId)
        {
            var pieChart = await _analyticsManager.GetTicketClosingByRate(entityType, entityId);
            return Ok(pieChart);
        }

        [HttpGet("analytics/GetOpenTicketStatusRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while calculating ticket status rates")]
        public async Task<IActionResult> GetOpenTicketStatusRate(
            string entityType,
            string entityId)
        {
            var pieChart = await _analyticsManager.GetOpenTicketStatusRate(entityType, entityId);
            return Ok(pieChart);
        }
        
        [HttpGet("analytics/GetAmountOfCreatedTicketsOfEntity/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the amount of tickets of entity")]
        public async Task<IActionResult> GetAmountOfCreatedTicketsOfEntity(
            string entityType,
            string entityId)
        {
            var amount = await _analyticsManager.GetAmountOfCreatedTickets(entityType, entityId);
            return Ok(amount);
        }

        [HttpGet("analytics/GetAmountOfClosedTickets/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the amount of closed tickets")]
        public async Task<IActionResult> GetAmountOfClosedTickets(
            string entityType,
            string entityId)
        {
            var amount = await _analyticsManager.GetAmountOfClosedTickets(entityType, entityId);
            return Ok(amount);
        }

        [HttpGet("analytics/GetAmountOfOpenTickets/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the amount of open tickets")]
        public async Task<IActionResult> GetAmountOfOpenTickets(
            string entityType,
            string entityId)
        {
            var amount = await _analyticsManager.GetAmountOfOpenTickets(entityType, entityId);
            return Ok(amount);
        }
        
        [HttpGet("analytics/GetAmountOfTicketsClosedByUserThatWereReopenedAfterwards/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the amount of \"superficially\" closed tickets")]
        public async Task<IActionResult> GetAmountOfTicketsClosedByUserThatWereReopenedAfterwards(string userId)
        {
            var amount = await _analyticsManager.GetAmountOfTicketsClosedByUserThatWereReopenedAfterwards(userId);
            return Ok(amount);
        }
        
        [HttpGet("analytics/GetAmountOfTicketsEverClosedByUser/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the amount tickets ever closed by user")]
        public async Task<IActionResult> GetAmountOfTicketsEverClosedByUser(string userId)
        {
            var amount = await _analyticsManager.GetAmountOfTicketsEverClosedByUser(userId);
            return Ok(amount);
        }

        [HttpGet("analytics/GetAmountOfLastCreatedTickets")]
        [DefaultExceptionHandler("An error occured while fetching the amount of last created tickets")]
        public async Task<IActionResult> GetAmountOfLastCreatedTickets(DateTime start, DateTime end, string interval)
        {
            PieChartData pieChart = await _analyticsManager.GetAmountOfLastCreatedTickets(start, end, interval);
            return Ok(pieChart);
        }

        [HttpGet("analytics/GetAmountOfLastClosedTickets")]
        [DefaultExceptionHandler("An error occured while fetching the amount of last closed tickets")]
        public async Task<IActionResult> GetAmountOfLastClosedTickets(DateTime start, DateTime end, string interval)
        {
            PieChartData pieChart = await _analyticsManager.GetAmountOfLastClosedTickets(start, end, interval);
            return Ok(pieChart);
        }
    }
}
