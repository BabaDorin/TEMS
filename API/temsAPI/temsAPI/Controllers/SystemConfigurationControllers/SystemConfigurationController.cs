using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Services;
using temsAPI.Services.SICServices;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.SystemConfiguration;

namespace temsAPI.Controllers.SystemConfigurationControllers
{
    public class SystemConfigurationController : TEMSController
    {
        private SystemConfigurationService _configService;

        public SystemConfigurationController(
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            SystemConfigurationService configService,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _configService = configService;
        }

        [HttpGet("systemconfiguration/IntegrateSIC")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while integrating SIC")]
        public async Task<IActionResult> IntegrateSIC()
        {
            var sicIntegrationService = new SIC_IntegrationService(_unitOfWork);
            await sicIntegrationService.PrepareDBForSICIntegration();
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("systemconfiguration/SetLibraryAllocatedStorageSpace/{gb}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while setting the library allocated storage space")]
        public IActionResult SetLibraryAllocatedStorageSpace(int gb)
        {
            var result = _configService.SetLibraryAllocatedStorageSpace(gb);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost("systemconfiguration/SetEmailSender")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while setting the Email sender")]
        public IActionResult SetEmailSender(EmailSenderCredentialsViewModel viewModel)
        {
            string result = _configService.SetEmailSender(viewModel.Address, viewModel.Password);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("systemconfiguration/SetRoutineCheckInterval/{hours}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while setting the interval value")]
        public IActionResult SetRoutineCheckInterval(int hours)
        {
            string result = _configService.SetRoutineCheckInterval(hours);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("systemconfiguration/SetArchieveInterval/{hours}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while setting the interval value")]
        public IActionResult SetArchieveInterval(int hours)
        {
            string result = _configService.SetArchieveIntervalHr(hours);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("systemconfiguration/SetGuestTicketCreationAllowance/{flag}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while setting the allowance status")]
        public IActionResult SetGuestTicketCreationAllowance(bool flag)
        {
            if (flag)
                _configService.AllowGuestsToCreateTickets();
            else
                _configService.ForbidGuestsToCreateTickets();

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("systemconfiguration/GetAppSettingsModel")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while getting the system configuration")]
        public IActionResult GetAppSettingsModel()
        {
            var viewModel = AppSettingsViewModel.FromModel(_configService.AppSettings);
            return Ok(viewModel);
        }

        [HttpGet("systemconfiguration/SetLibraryPassword/{password}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while setting the library password")]
        public IActionResult SetLibraryPassword(string password)
        {
            _configService.SetLibraryGuestPass(password);
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("systemconfiguration/GetLibraryPassword")]
        public JsonResult GetLibraryPassword()
        {
            try
            {
                return Json(_configService.GetLibraryPasswordMd5());
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
    }
}
