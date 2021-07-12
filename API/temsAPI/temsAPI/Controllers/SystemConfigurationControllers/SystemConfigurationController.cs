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
using temsAPI.ViewModels.SystemConfiguration;

namespace temsAPI.Controllers.SystemConfigurationControllers
{
    public class SystemConfigurationController : TEMSController
    {
        private SystemConfigurationService _configService;

        public SystemConfigurationController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            SystemConfigurationService configService,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _configService = configService;
        }

        [HttpGet("systemconfiguration/IntegrateSIC")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<IActionResult> IntegrateSIC()
        {
            try
            {
                var sicIntegrationService = new SIC_IntegrationService(_unitOfWork);
                await sicIntegrationService.PrepareDBForSICIntegration();
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while integrating SIC", ResponseStatus.Fail);
            }

        }

        [HttpGet("systemconfiguration/SetLibraryAllocatedStorageSpace/{gb}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public IActionResult SetLibraryAllocatedStorageSpace(int gb)
        {
            try
            {
                var result = _configService.SetLibraryAllocatedStorageSpace(gb);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while setting the library allocated storage space", ResponseStatus.Fail);
            }
        }

        [HttpPost("systemconfiguration/SetEmailSender")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public IActionResult SetEmailSender(EmailSenderCredentialsViewModel viewModel)
        {
            try
            {
                string result = _configService.SetEmailSender(viewModel.Address, viewModel.Password);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while setting the Email sender.", ResponseStatus.Fail);
            }
        }

        [HttpGet("systemconfiguration/SetRoutineCheckInterval/{hours}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public IActionResult SetRoutineCheckInterval(int hours)
        {
            try
            {
                string result = _configService.SetRoutineCheckInterval(hours);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while setting the interval value.", ResponseStatus.Fail);
            }
        }

        [HttpGet("systemconfiguration/SetArchieveInterval/{hours}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public IActionResult SetArchieveInterval(int hours)
        {
            try
            {
                string result = _configService.SetArchieveIntervalHr(hours);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while setting the interval value.", ResponseStatus.Fail);
            }
        }

        [HttpGet("systemconfiguration/SetGuestTicketCreationAllowance/{flag}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public IActionResult SetGuestTicketCreationAllowance(bool flag)
        {
            try
            {
                if (flag)
                    _configService.AllowGuestsToCreateTickets();
                else
                    _configService.ForbidGuestsToCreateTickets();

                return ReturnResponse("Success", ResponseStatus.Fail);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while setting the allowance status.", ResponseStatus.Fail);
            }
        }

        [HttpGet("systemconfiguration/GetAppSettingsModel")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public IActionResult GetAppSettingsModel()
        {
            try
            {
                var viewModel = AppSettingsViewModel.FromModel(_configService.AppSettings);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while getting the system configuration", ResponseStatus.Fail);
            }
        }

        [HttpGet("systemconfiguration/SetLibraryPassword/{password}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public IActionResult SetLibraryPassword(string password)
        {
            try
            {
                _configService.SetLibraryGuestPass(password);
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while setting the library password", ResponseStatus.Fail);
            }
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
                //return ReturnResponse("An error occured getting the library password", ResponseStatus.Fail);
            }
        }
    }
}
