﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class TEMSUserController : TEMSController
    {
        private RoleManager<IdentityRole> _roleManager;
        private readonly AppSettings _appSettings;
        private UserHelper _userHelper;

        public TEMSUserController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings) : base(mapper, unitOfWork, userManager)
        {
            _roleManager = roleManager;
            _appSettings = appSettings.Value;

            _userHelper = new UserHelper(unitOfWork, userManager, roleManager);
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
                    FullName =
                        String.IsNullOrEmpty(viewModel.FullName)
                        ? viewModel.Username
                        : viewModel.FullName,
                    PhoneNumber = viewModel.PhoneNumber,
                    Email = viewModel.Email,
                };

                // Creating the user
                string result = await _userHelper.CreateUser(model, viewModel.Password);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                // Assigning roles
                result = await _userHelper.AssignRoles(model, viewModel.Roles);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                // Setting claims
                result = await _userHelper.SetClaims(model, viewModel.Claims);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                // Creating Personnel-User Association
                result = await _userHelper.UserPersonnelAssociation(model, viewModel.Personnel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);
                
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
        public async Task<JsonResult> UpdateUser([FromBody] AddUserViewModel viewModel)
        {
            try
            {
                string result = null;

                // Invalid id
                if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == viewModel.Id))
                    return ReturnResponse("Invalid Id provided", ResponseStatus.Fail);

                // Invalid username
                viewModel.Username = viewModel.Username?.Trim();
                if (String.IsNullOrEmpty(viewModel.Username))
                    return ReturnResponse("Invalid username provided", ResponseStatus.Fail);

                var model = await _userManager.FindByIdAsync(viewModel.Id);

                // Password reset (If needed)
                if(viewModel.Password != null)
                {
                    result = await _userHelper.ResetPassword(model, viewModel.Password);
                    if (result != null)
                        return ReturnResponse(result, ResponseStatus.Fail);
                }

                model.UserName = viewModel.Username;
                model.PhoneNumber = viewModel.PhoneNumber;
                model.Email = viewModel.Email;
                model.FullName = viewModel.FullName;

                // Updating properties
                result = await _userHelper.UpdateUserData(model);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                // Assigning roles
                result = await _userHelper.AssignRoles(model, viewModel.Roles);
                if(result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                // Setting claims
                result = await _userHelper.SetClaims(model, viewModel.Claims);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                // Creating Personnel-Personnel Association
                result = await _userHelper.UserPersonnelAssociation(model, viewModel.Personnel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("The user has been saved", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when updating the record", ResponseStatus.Fail);
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
                        new Claim("UserID", user.Id.ToString()),
                        new Claim("Username", user.UserName.ToString()),
                    }),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Ok(new { token });
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            try
            {
                List<ViewUserSimplifiedViewModel> viewModel = _userManager
                    .Users
                    .Where(q => !q.IsArchieved)
                    .Select(q => new ViewUserSimplifiedViewModel
                    {
                        Username = q.UserName,
                        Email = q.Email,
                        FullName = q.FullName,
                        Id = q.Id,
                        //Roles = _userManager.GetRolesAsync(q).Result.Count().ToString(),
                    }).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching users", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetClaims()
        {
            try
            {
                List<Option> claims = (await _unitOfWork.Privileges.FindAll<Option>(
                    select: q => new Option
                    {
                        Value = q.Identifier,
                        Label = q.Identifier,
                        Additional = q.Description
                    }
                    )).ToList();

                return Json(claims);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getroleclaims/{roles}")]
        public async Task<JsonResult> GetRoleClaims(string roles)
        {
            try
            {
                if (roles == null)
                    return Json(new List<string>());

                List<string> rolesList = roles.Split(",").ToList();

                List<string> claims = new List<string>();
                foreach (var role in rolesList)
                {
                    var roleClaims = await _roleManager
                        .GetClaimsAsync(await _roleManager.FindByNameAsync(role));

                    claims = claims.Union(roleClaims.Select(q => q.Type)).ToList();
                }

                return Json(claims);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching role claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getuserclaims/{userId}")]
        public async Task<JsonResult> GetUserClaims(string userId)
        {
            try
            {
                // Invalid id provided
                if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == userId))
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                List<string> claims = (await _userManager
                    .GetClaimsAsync(await _userManager.FindByIdAsync(userId)))
                    .Select(q => q.Type)
                    .ToList();

                if (claims == null) claims = new List<string>();
                return Json(claims);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching user claims", ResponseStatus.Fail);
            }
        }

        [HttpGet("temsuser/getuser/{userId}")]
        public async Task<JsonResult> GetUser(string userId)
        {
            try
            {
                var user = (await _unitOfWork.TEMSUsers
                    .Find<TEMSUser>(
                        where: q => q.Id == userId,
                        include: q => q
                        .Include(q => q.Personnel)
                    )).FirstOrDefault();

                var viewModel = new ViewUserViewModel
                {
                    Id = user.Id,
                    Claims = _userManager.GetClaimsAsync(user)
                    .Result?
                    .Select(q => q.Type)
                    .ToList(),
                    Email = user.Email,
                    FullName = user.FullName,
                    Personnel = user.Personnel == null
                        ? null
                        : new Option
                        {
                            Value = user.PersonnelId,
                            Label = user.Personnel.Name
                        },
                    PhoneNumber = user.PhoneNumber,
                    Username = user.UserName,
                    Roles = _userManager.GetRolesAsync(user).Result?.ToList(),
                };

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching user", ResponseStatus.Fail);
            }
        }
    }
}