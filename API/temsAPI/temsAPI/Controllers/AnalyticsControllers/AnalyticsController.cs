using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            CurrencyConvertor currencyConvertor) : base(mapper, unitOfWork, userManager)
        {
            _analyticsManager = analyticsManager;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("analytics/getEquipmentAmount/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/getEquipmentTotalCost/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/getEquipmentUtilizationRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }

        }

        [HttpGet("analytics/getEquipmentTypeRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }

        }

        [HttpGet("analytics/getEquipmentAllocationRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/getEquipmentWorkabilityRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/getTicketClosingRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/getTicketClosingByRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }

        [HttpGet("analytics/getOpenTicketStatusRate/{entityType?}/{entityId?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching analytics", ResponseStatus.Fail);
            }
        }
    }
}
