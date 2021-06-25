using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class AuthController : TEMSController
    {
        TEMSUserManager _temsUserManager;
        RoleManager<IdentityRole> _roleManager;
        readonly AppSettings _appSettings;
        public AuthController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings,
            TEMSUserManager temsUserManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _roleManager = roleManager;
            _appSettings = appSettings.Value;
            _temsUserManager = temsUserManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignOut([FromBody] string token)
        {
            try
            {
                await _temsUserManager.BlacklistToken(token);
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while blacklisting the token", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] LogInViewModel viewModel)
        {
            try
            {
                var token = await _temsUserManager.GenerateToken(viewModel);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("Invalid credentials", ResponseStatus.Fail);
            }
        }
    }
}
