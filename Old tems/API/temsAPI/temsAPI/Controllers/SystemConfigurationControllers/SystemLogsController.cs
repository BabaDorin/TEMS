using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.SystemConfiguration;

namespace temsAPI.Controllers.SystemConfigurationControllers
{
    public class SystemLogsController : TEMSController
    {
        readonly SystemConfigurationService _configService;

        public SystemLogsController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager, 
            ILogger<TEMSController> logger,
            SystemConfigurationService systemConfigurationService) : base(unitOfWork, userManager, logger)
        {
            _configService = systemConfigurationService;
        }

        [HttpPost("systemlogs/GetSystemLogs")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while retrieving system logs")]
        public JsonResult GetSystemLogs([FromBody] LoggerViewModel viewModel)
        {
            return Json(_configService.GetLogsByDate(viewModel.Date));
        }
    }
}
