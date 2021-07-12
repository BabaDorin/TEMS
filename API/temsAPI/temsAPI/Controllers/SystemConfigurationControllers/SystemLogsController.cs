using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels.SystemConfiguration;

namespace temsAPI.Controllers.SystemConfigurationControllers
{
    public class SystemLogsController : TEMSController
    {
        SystemConfigurationService _configService;

        public SystemLogsController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager, 
            ILogger<TEMSController> logger,
            SystemConfigurationService systemConfigurationService) : base(mapper, unitOfWork, userManager, logger)
        {
            _configService = systemConfigurationService;
        }

        [HttpPost("systemlogs/GetSystemLogs")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult GetSystemLogs([FromBody] LoggerViewModel viewModel)
        {
            try
            {
                return Json(_configService.GetLogsByDate(viewModel.Date));
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
    }
}
