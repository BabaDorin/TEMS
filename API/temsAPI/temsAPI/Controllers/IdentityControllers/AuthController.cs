using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class AuthController : TEMSController
    {
        TEMSUserManager _temsUserManager;

        public AuthController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            TEMSUserManager temsUserManager,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _temsUserManager = temsUserManager;
        }

        [HttpPost("auth/LogIn")]
        [DefaultExceptionHandler("Invalid credentials", true)]
        public async Task<IActionResult> LogIn([FromBody] LogInViewModel viewModel)
        {
            var token = await _temsUserManager.GenerateToken(viewModel);
            return Ok(new { token });
        }

        [HttpPost("auth/SignOut")]
        [DefaultExceptionHandler("An error occured while blacklisting the token")]
        public async Task<IActionResult> SignOut([FromBody] string token)
        {
            await _temsUserManager.BlacklistToken(token);
            return ReturnResponse("Success", ResponseStatus.Success);
        }
    }
}
