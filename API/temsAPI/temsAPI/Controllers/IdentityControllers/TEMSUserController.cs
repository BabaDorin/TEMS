using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class TEMSUserController : TEMSController
    {
        TEMSUserManager _temsUserManager;
        private ArchieveManager _archieveManager;

        public TEMSUserController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            TEMSUserManager temsUserManager,
            IdentityService identityService,
            ArchieveManager archieveManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _archieveManager = archieveManager;
            _temsUserManager = temsUserManager;
        }

        [HttpPost("temsuser/AddUser")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> AddUser([FromBody] AddUserViewModel viewModel)
        {
            try
            {
                string result = await _temsUserManager.CreateUser(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse(
                    "An error occured when saving the user, consider trying again",
                    ResponseStatus.Fail);
            }
        }

        [HttpPut("temsuser/UpdateUser")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> UpdateUser([FromBody] AddUserViewModel viewModel)
        {
            try
            {
                var result = await _temsUserManager.UpdateUser(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when updating the record", ResponseStatus.Fail);
            }
        }

        [HttpDelete("temsuser/Remove/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> Remove(string userId)
        {
            try
            {
                string result = await _temsUserManager.RemoveUser(userId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the user.", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/Archieve/{userId}/{status}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> Archieve(string userId, bool status = true)
        {
            try
            {
                string result = await _archieveManager.SetArchivationStatus("user", userId, true);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while archieving the user.", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/GetAllAutocompleteOptions/{filter?}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(string filter)
        {
            try
            {
                var options = await _temsUserManager.GetAutocompleteOptions(filter);
                return Json(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/GetUsers/{role?}")]
        public async Task<JsonResult> GetUsers(string role)
        {
            try
            {
                var users = await _temsUserManager.GetUsers(role);
                return Json(users);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching users", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/GetClaims")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> GetClaims()
        {
            try
            {
                var claims = await _temsUserManager.GetClaims();
                return Json(claims);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/GetRoleClaims/{roles}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> GetRoleClaims(string roles)
        {
            try
            {
                if (roles == null)
                    return Json(new List<string>());

                List<string> rolesList = roles.Split(",").ToList();
                
                var claims = await _temsUserManager.GetRolesCumulativeClaims(rolesList);
                return Json(claims);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching role claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/GetUserClaims/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> GetUserClaims(string userId)
        {
            try
            {
                var claims = await _temsUserManager.GetUserClaims(userId);
                if (claims == null) 
                    claims = new List<string>();

                return Json(claims);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching user claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/GetUser/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetUser(string userId)
        {
            try
            {
                var userViewModel = await _temsUserManager.GetUser(userId);
                return Json(userViewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching user", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/GetSimplifiedById/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetSimplifiedById(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                return Json(ViewUserSimplifiedViewModel.FromModel(user, _userManager));
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching users", ResponseStatus.Fail);
            }
        }

        [HttpPut("temsuser/ChangePassword")]
        [Authorize]
        public async Task<JsonResult> ChangePassword([FromBody] ChangePasswordViewModel viewModel)
        {
            try
            {
                var result = await _temsUserManager.ChangePassword(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while changing account's password", ResponseStatus.Fail);
            }
        }

        [HttpPut("temsuser/ChangeEmailPreferences")]
        [Authorize]
        public async Task<JsonResult> ChangeEmailPreferences([FromBody] ChangeEmailPreferencesViewModel viewModel)
        {
            try
            {
                var result = await _temsUserManager.ChangeEmailPreferences(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while saving email preferences.", ResponseStatus.Fail);
            }
        }

        [HttpPut("temsuser/EditAccountGeneralInfo")]
        [Authorize]
        public async Task<JsonResult> EditAccountGeneralInfo([FromBody] AccountGeneralInfoViewModel viewModel)
        {
            try
            {
                var result = await _temsUserManager.EditAccountInfo(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while updating account information", ResponseStatus.Fail);
            }
        }
    }
}
