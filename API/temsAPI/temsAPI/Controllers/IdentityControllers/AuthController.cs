using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class AuthController : TEMSController
    {
        RoleManager<IdentityRole> _roleManager;
        readonly AppSettings _appSettings;
        public AuthController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings) : base(mapper, unitOfWork, userManager)
        {
            _roleManager = roleManager;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public JsonResult IsAuthenticated()
        {
            return ReturnResponse("yep", ResponseStatus.Success);
        }

        [HttpPost]
        public async Task<IActionResult> SignOut([FromBody] string token)
        {
            try
            {
                // User's jwt is being blacklisted here.
                await _unitOfWork.JWTBlacklist.Create(
                    new TemsJWT
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = token,
                        ExpirationDate = (new JWTHelper()).GetExpiryTimestamp(token)
                    });

                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when invalidating token", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] LogInViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(viewModel.Username);
            if(user == null) 
                user = await _userManager.FindByEmailAsync(viewModel.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, viewModel.Password))
                return ReturnResponse("Username or password is incorrect", ResponseStatus.Fail);

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("UserID", user.Id.ToString()));
            claims.Add(new Claim("Username", user.UserName.ToString()));

            var userClaims = await _userManager.GetClaimsAsync(user);

            List<string> userRoles = (await _userManager.GetRolesAsync(user)).ToList();
            foreach (string role in userRoles)
            {
                userClaims = userClaims.Union(await _roleManager
                    .GetClaimsAsync(await _roleManager.FindByNameAsync(role)))
                    .Select(q => new Claim(q.Type, q.Value))
                    .ToList();
            }

            foreach (var claim in userClaims)
            {
                claims.Add(new Claim(claim.Type, claim.Value));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            _appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Ok(new { token });
        }
    }
}
