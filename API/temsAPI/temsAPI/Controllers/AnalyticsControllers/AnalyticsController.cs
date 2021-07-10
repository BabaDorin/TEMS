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
        public async Task<JsonResult> GetEquipmentAmount(
            string entityType = null,
            string entityId = null)
        {
            try
            {
                var totalNumberOfEquipment = await _analyticsManager.GetEquipmentAmount(entityType, entityId);
                return Json(totalNumberOfEquipment);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetEquipmentTotalCost/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetEquipmentTotalCost(
            string entityType = null,
            string entityId = null)
        {
            try
            {
                double totalCost = await _analyticsManager.GetEquipmentTotalCost(entityType, entityId);
                return Json(Math.Round(totalCost, 2));
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetEquipmentUtilizationRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetEquipmentUtilizationRate(
            string entityType = null,
            string entityId = null)
        {
            try
            {
                var pieChart = await _analyticsManager.GetEquipmentUtilizationRate(entityType, entityId);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }

        }

        [HttpGet("analytics/GetEquipmentTypeRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetEquipmentTypeRate(
            string entityType = null,
            string entityId = null)
        {
            try
            {
                var pieChart = await _analyticsManager.GetEquipmentTypeRate(entityType, entityId);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }

        }

        [HttpGet("analytics/GetEquipmentAllocationRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetEquipmentAllocationRate(
            string entityType = null,
            string entityId = null)
        {
            try
            {
                var pieChart = await _analyticsManager.GetEquipmentAllocationRate(entityType, entityId);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetEquipmentWorkabilityRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetEquipmentWorkabilityRate(
            string entityType = null,
            string entityId = null)
        {
            try
            {
                var pieChart = await _analyticsManager.GetEquipmentWorkabilityRate(entityType, entityId);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetTicketClosingRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetTicketClosingRate(
            string entityType,
            string entityId)
        {
            try
            {
                var pieChart = await _analyticsManager.GetTicketClosingRate(entityType, entityId);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetTicketClosingByRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetTicketClosingByRate(
            string entityType,
            string entityId)
        {
            try
            {
                var pieChart = await _analyticsManager.GetTicketClosingByRate(entityType, entityId);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetOpenTicketStatusRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetOpenTicketStatusRate(
            string entityType,
            string entityId)
        {
            try
            {
                var pieChart = await _analyticsManager.GetOpenTicketStatusRate(entityType, entityId);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }
        
        [HttpGet("analytics/GetAmountOfCreatedTicketsOfEntity/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetAmountOfCreatedTicketsOfEntity(
            string entityType,
            string entityId)
        {
            try
            {
                var amount = await _analyticsManager.GetAmountOfCreatedTickets(entityType, entityId);
                return Json(amount);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetAmountOfClosedTickets/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetAmountOfClosedTickets(
            string entityType,
            string entityId)
        {
            try
            {
                var amount = await _analyticsManager.GetAmountOfClosedTickets(entityType, entityId);
                return Json(amount);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetAmountOfOpenTickets/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetAmountOfOpenTickets(
            string entityType,
            string entityId)
        {
            try
            {
                var amount = await _analyticsManager.GetAmountOfOpenTickets(entityType, entityId);
                return Json(amount);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }
        
        [HttpGet("analytics/GetAmountOfTicketsClosedByUserThatWereReopenedAfterwards/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetAmountOfTicketsClosedByUserThatWereReopenedAfterwards(string userId)
        {
            try
            {
                var amount = await _analyticsManager.GetAmountOfTicketsClosedByUserThatWereReopenedAfterwards(userId);
                return Json(amount);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }
        
        [HttpGet("analytics/GetAmountOfTicketsEverClosedByUser/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetAmountOfTicketsEverClosedByUser(string userId)
        {
            try
            {
                var amount = await _analyticsManager.GetAmountOfTicketsEverClosedByUser(userId);
                return Json(amount);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetAmountOfLastCreatedTickets")]
        public async Task<JsonResult> GetAmountOfLastCreatedTickets(DateTime start, DateTime end, string interval)
        {
            try
            {
                PieChartData pieChart = await _analyticsManager.GetAmountOfLastCreatedTickets(start, end, interval);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/GetAmountOfLastClosedTickets")]
        public async Task<JsonResult> GetAmountOfLastClosedTickets(DateTime start, DateTime end, string interval)
        {
            try
            {
                PieChartData pieChart = await _analyticsManager.GetAmountOfLastClosedTickets(start, end, interval);
                return Json(pieChart);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }
    }
}
