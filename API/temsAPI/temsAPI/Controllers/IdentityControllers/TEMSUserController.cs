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
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class TEMSUserController : TEMSController
    {
        private RoleManager<IdentityRole> _roleManager;
        private readonly AppSettings _appSettings;

        public TEMSUserController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings) : base(mapper, unitOfWork, userManager)
        {
            _roleManager = roleManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<JsonResult> AddUser([FromBody] AddUserViewModel viewModel)
        {
            try
            {
                // Username and password are mandatory
                viewModel.Username = viewModel.Username?.Trim();
                if (String.IsNullOrEmpty(viewModel.Username) || String.IsNullOrEmpty(viewModel.Password))
                    return ReturnResponse("Invalid username or password provided", ResponseStatus.Fail);

                TEMSUser model = new TEMSUser()
                {
                    UserName = viewModel.Username,
                    PhoneNumber = viewModel.PhoneNumber,
                    Email = viewModel.Email,
                };

                // Creating the user
                var result = await _userManager.CreateAsync(model, viewModel.Password);

                // Errors when creating the user
                if (result.Errors.Count() > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var error in result.Errors)
                    {
                        stringBuilder.Append(error.Description + "\n");
                    }
                    return ReturnResponse(stringBuilder.ToString(), ResponseStatus.Fail);
                }

                // Assigning roles
                result = await _userManager.AddToRoleAsync(model, "User");

                if (viewModel.Roles.Count > 0)
                {
                    List<string> roleIds = viewModel
                        .Roles
                        .Select(q => q.Value)
                        .ToList();

                    List<string> roles = _roleManager
                        .Roles
                        .Where(q => roleIds.Contains(q.Id) && q.Name != "User")
                        .Select(q => q.Name)
                        .ToList();

                    result = await _userManager.AddToRolesAsync(model, roles);
                }

                // Errors when assigning roles
                if (result.Errors.Count() > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var error in result.Errors)
                    {
                        stringBuilder.Append(error.Description + "\n");
                    }
                    return ReturnResponse(stringBuilder.ToString(), ResponseStatus.Fail);
                }

                // Creating User-Personnel Association
                if (viewModel.Personnel != null)
                {
                    if (!await _unitOfWork.Personnel
                        .isExists(q => q.Id == viewModel.Personnel.Value))
                    {
                        return ReturnResponse(
                            "The specified personnel record seems invalid",
                             ResponseStatus.Fail
                             );
                    }

                    model.Personnel = (await _unitOfWork
                        .Personnel
                        .Find<Personnel>(
                            q => q.Id == viewModel.Personnel.Value
                        )).FirstOrDefault();

                    result = await _userManager.UpdateAsync(model);

                    if (result.Errors.Count() > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var error in result.Errors)
                        {
                            stringBuilder.Append(error.Description + "\n");
                        }
                        return ReturnResponse(stringBuilder.ToString(), ResponseStatus.Fail);
                    }
                }

                return ReturnResponse("The user has been saved", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse(
                    "An error occured when saving the user, consider trying again",
                    ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] LogInViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(viewModel.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, viewModel.Password))
                return ReturnResponse("Username or password is incorrect", ResponseStatus.Fail);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString())
                    }),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Ok(new { token });
        }
    }
}
