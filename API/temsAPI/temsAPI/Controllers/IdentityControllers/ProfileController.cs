using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;

namespace temsAPI.Controllers.IdentityControllers
{
    [Authorize]
    public class ProfileController : TEMSController
    {
        private TEMSUserManager _temsUserManager;

        public ProfileController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            TEMSUserManager temsUserManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
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
    }
}
