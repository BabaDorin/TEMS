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
            SystemConfigurationService configService) : base(mapper, unitOfWork, userManager)
        {
            _configService = configService;
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> IntegrateSIC()
        {
            try
            {
                var sicIntegrationService = new SIC_IntegrationService(_unitOfWork);
                await sicIntegrationService.PrepareDBForSICIntegration();
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while integrating SIC", ResponseStatus.Fail);
            }

        }

        [HttpGet("systemconfiguration/setlibraryallocatedstoragespace/{gb}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult SetLibraryAllocatedStorageSpace(int gb)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while setting the library allocated storage space", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult SetEmailSender(EmailSenderCredentialsViewModel viewModel)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while setting the Email sender.", ResponseStatus.Fail);
            }
        }

        [HttpGet("systemconfiguration/setroutinecheckinterval/{hours}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult SetRoutineCheckInterval(int hours)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while setting the interval value.", ResponseStatus.Fail);
            }
        }

        [HttpGet("systemconfiguration/setarchieveinterval/{hours}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult SetArchieveInterval(int hours)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while setting the interval value.", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult SetGuestTicketCreationAllowance(bool flag)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while setting the allowance status.", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult GetAppSettingsModel()
        {
            try
            {
                var viewModel = AppSettingsViewModel.FromModel(_configService.AppSettings);
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while getting the system configuration", ResponseStatus.Fail);
            }
        }

        [HttpGet("systemconfiguration/setlibrarypassword/{password}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult SetLibraryPassword(string password)
        {
            try
            {
                _configService.SetLibraryGuestPass(password);
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while setting the library password", ResponseStatus.Fail);
            }
        }

    }
}
