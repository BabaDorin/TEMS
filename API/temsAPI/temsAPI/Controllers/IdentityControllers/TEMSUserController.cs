using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class TEMSUserController : TEMSController
    {
        private RoleManager<IdentityRole> _roleManager;
        private IdentityService _identityService;
        TEMSUserManager _temsUserManager;

        public TEMSUserController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            TEMSUserManager temsUserManager,
            IdentityService identityService) : base(mapper, unitOfWork, userManager)
        {
            _roleManager = roleManager;
            _temsUserManager = temsUserManager;
            _identityService = identityService;
        }

        [HttpPost]
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
                Debug.WriteLine(ex);
                return ReturnResponse(
                    "An error occured when saving the user, consider trying again",
                    ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/removeUser/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> RemoveUser(string userId)
        {
            try
            {
                var result = await _temsUserManager.RemoveUser(userId);
                if(result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while removing the user", ResponseStatus.Fail);
            }
        }

        [HttpPost]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when updating the record", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getallautocompleteoptions/{filter?}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(string filter)
        {
            try
            {
                var options = await _temsUserManager.GetAutocompleteOptions(filter);
                return Json(options);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getusers/{role?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> GetUsers(string role)
        {
            try
            {
                var users = await _temsUserManager.GetUsers(role);
                return Json(users);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching users", ResponseStatus.Fail);
            }
        }

        [HttpGet]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getroleclaims/{roles}")]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching role claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getuserclaims/{userId}")]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching user claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getuser/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetUser(string userId)
        {
            try
            {
                var userViewModel = await _temsUserManager.GetUser(userId);
                return Json(userViewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching user", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getsimplifiedbyid/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplifiedById(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                return Json(ViewUserSimplifiedViewModel.FromModel(user, _userManager));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching users", ResponseStatus.Fail);
            }
        }

        [HttpPost]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while changing account's password", ResponseStatus.Fail);
            }
        }

        [HttpPost]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while saving email preferences.", ResponseStatus.Fail);
            }
        }

        [HttpPost]
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while updating account information", ResponseStatus.Fail);
            }
        }
    }
}
