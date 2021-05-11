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
using temsAPI.Services;
using temsAPI.System_Files;

namespace temsAPI.Controllers.AnalyticsControllers
{
    public class AnalyticsController : TEMSController
    {
        private AnalyticsManager _analyticsManager;
        private CurrencyConvertor _currencyConvertor;

        public AnalyticsController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            AnalyticsManager analyticsManager,
            CurrencyConvertor currencyConvertor) : base(mapper, unitOfWork, userManager)
        {
            _analyticsManager = analyticsManager;
            _currencyConvertor = currencyConvertor;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetEquipmentAmount(
            string entityType = null, 
            string entityId = null)
        {
            try
            {
                var totalNumberOfEquipment = await _analyticsManager.GetEquipmentAmount(
                entityType,
                entityId);

                return Json(totalNumberOfEquipment);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while getting equipment analytics", ResponseStatus.Fail);
            }
        } 


    }
}
