using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;

namespace temsAPI.Controllers.IdentityControllers
{
    [Authorize]
    public class ProfileController : TEMSController
    {
        readonly TEMSUserManager _temsUserManager;

        public ProfileController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            TEMSUserManager temsUserManager,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _temsUserManager = temsUserManager;
        }

        [HttpGet("profile/Get/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching user data")]
        public async Task<IActionResult> Get(string userId)
        {
            var profileViewModel = await _temsUserManager.GetProfileInfo(userId);
            return Ok(profileViewModel);
        }

        [HttpGet("profile/getMinifiedProfilePhoto")]
        [Authorize]
        public async Task<JsonResult> GetMinifiedProfilePhoto(string userId = null)
        {
            try
            {
                string actualUserId = userId ?? IdentityService.GetUserId(User);

                var user = await _userManager.FindByIdAsync(actualUserId);
                if (user == null)
                    throw new System.Exception("invalid userId provided");

                ProfilePhotoHandler handler = new();
                return Json(handler.GetMinifiedProfilePhotoBase64(user));
            }
            catch (Exception ex)
            {
                LogException(ex);
                return null;
            }
        }
    }
}
