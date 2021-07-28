using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
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
        [DefaultExceptionHandler("An error occured while saving the user, consider trying again")]
        public async Task<IActionResult> AddUser([FromBody] AddUserViewModel viewModel)
        {
            string result = await _temsUserManager.CreateUser(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPut("temsuser/UpdateUser")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured when updating the record")]
        public async Task<IActionResult> UpdateUser([FromBody] AddUserViewModel viewModel)
        {
            var result = await _temsUserManager.UpdateUser(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("temsuser/Remove/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the user")]
        public async Task<IActionResult> Remove(string userId)
        {
            string result = await _temsUserManager.RemoveUser(userId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("temsuser/Archieve/{userId}/{status}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while archieving the user")]
        public async Task<IActionResult> Archieve(string userId, bool status = true)
        {
            string result = await _archieveManager.SetArchivationStatus("user", userId, true);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("temsuser/GetAllAutocompleteOptions/{filter?}")]
        [DefaultExceptionHandler("An error occured when fetching autocomplete options")]
        public async Task<IActionResult> GetAllAutocompleteOptions(string filter)
        {
            var options = await _temsUserManager.GetAutocompleteOptions(filter);
            return Ok(options);
        }

        [HttpGet("temsuser/GetUsers/{role?}")]
        [DefaultExceptionHandler("An error occured while fetching users")]
        public async Task<IActionResult> GetUsers(string role)
        {
            var users = await _temsUserManager.GetUsers(role);
            return Ok(users);
        }

        [HttpGet("temsuser/GetClaims")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured when fetching claims")]
        public async Task<IActionResult> GetClaims()
        {
            var claims = await _temsUserManager.GetClaims();
            return Ok(claims);
        }

        [HttpGet("temsuser/GetRoleClaims/{roles}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured when fetching role claims")]
        public async Task<IActionResult> GetRoleClaims(string roles)
        {
            if (roles == null)
                return Ok(new List<string>());

            List<string> rolesList = roles.Split(",").ToList();

            var claims = await _temsUserManager.GetRolesCumulativeClaims(rolesList);
            return Ok(claims);
        }

        [HttpGet("temsuser/GetUserClaims/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured when fetching user claims")]
        public async Task<IActionResult> GetUserClaims(string userId)
        {
            var claims = await _temsUserManager.GetUserClaims(userId);
            if (claims == null)
                claims = new List<string>();

            return Ok(claims);
        }

        [HttpGet("temsuser/GetUser/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching user")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var userViewModel = await _temsUserManager.GetUser(userId);
            return Ok(userViewModel);
        }

        [HttpGet("temsuser/GetSimplifiedById/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching users")]
        public async Task<IActionResult> GetSimplifiedById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return Ok(ViewUserSimplifiedViewModel.FromModel(user, _userManager));
        }

        [HttpPut("temsuser/ChangePassword")]
        [Authorize]
        [DefaultExceptionHandler("An error occured while changing account's password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel viewModel)
        {
            var result = await _temsUserManager.ChangePassword(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPut("temsuser/ChangeEmailPreferences")]
        [Authorize]
        [DefaultExceptionHandler("An error occured while saving email preferences.")]
        public async Task<IActionResult> ChangeEmailPreferences([FromBody] ChangeEmailPreferencesViewModel viewModel)
        {
            var result = await _temsUserManager.ChangeEmailPreferences(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPut("temsuser/EditAccountGeneralInfo")]
        [Authorize]
        [DefaultExceptionHandler("An error occured while updating account information")]
        public async Task<IActionResult> EditAccountGeneralInfo([FromBody] AccountGeneralInfoViewModel viewModel)
        {
            var result = await _temsUserManager.EditAccountInfo(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }
    }
}
