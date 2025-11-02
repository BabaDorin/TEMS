using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Services.JWT;
using temsAPI.System_Files;
using temsAPI.ViewModels;

namespace temsAPI.Services
{
    public class IdentityService
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<TEMSUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private AppSettings _appSettings;
        private ClaimsPrincipal _user;
        private TokenValidatorService _tokenValidator;
        public ClaimsPrincipal User => _user;

        public IdentityService(
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings,
            ClaimsPrincipal user,
            TokenValidatorService tokenValidator)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _appSettings = appSettings.Value;
            _user = user;
            _tokenValidator = tokenValidator;
        }

        public async Task<TEMSUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(GetUserId());
        }

        public static string GetUserId(ClaimsPrincipal user)
        {
            return user.Claims.Where(q => q.Type == "UserID").Select(q => q.Value).SingleOrDefault();
        }

        public string GetUserId()
        {
            return _user.Claims.Where(q => q.Type == "UserID").Select(q => q.Value).SingleOrDefault();
        }

        public static bool isAuthenticated(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }

        public bool IsAuthenticated()
        {
            return _user.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Associates an user model with a personnel record
        /// </summary>
        /// <param name="model"></param>
        /// <param name="personnel"></param>
        /// <returns></returns>
        public async Task<string> UserPersonnelAssociation(TEMSUser model, Option personnel)
        {
            Personnel personnelToBeSet = null;

            if (personnel != null)
            {
                if (!await _unitOfWork.Personnel.isExists(q => q.Id == personnel.Value))
                    return "The specified personnel record seems invalid";

                personnelToBeSet = (await _unitOfWork
                    .Personnel
                    .Find<Personnel>(
                        q => q.Id == personnel.Value
                    )).FirstOrDefault();
            }

            model.Personnel = personnelToBeSet;
            var result = await _userManager.UpdateAsync(model);

            return CheckResultForErrors(result);
        }

        /// <summary>
        /// Removes all of user roles and assignes new ones
        /// </summary>
        /// <param name="model"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<string> AssignRoles(TEMSUser model, List<string> rolesToSet)
        {
            var currentRoles = await _userManager.GetRolesAsync(model);
            if (currentRoles == rolesToSet)
                return null;

            // Remove current roles
            var result = await _userManager.RemoveFromRolesAsync(
                model, await _userManager.GetRolesAsync(model));

            if (result.Errors.Count() > 0)
                return CheckResultForErrors(result);

            result = await _userManager.AddToRoleAsync(model, "User");

            if (rolesToSet.Count > 0)
            {
                // Protect agains invalid role names being sent
                List<string> actualRoles = _roleManager
                    .Roles
                    .Where(q => rolesToSet.Contains(q.Name) && q.Name != "User")
                    .Select(q => q.Name)
                    .ToList();

                result = await _userManager.AddToRolesAsync(model, actualRoles);
            }

            if (result.Errors.Count() > 0)
                return CheckResultForErrors(result);

            await _tokenValidator.BlacklistUserToken(model.Id);
            return null;
        }

        /// <summary>
        /// Creates an user entity
        /// </summary>
        /// <param name="model"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> CreateUser(TEMSUser model, string password)
        {
            // Creating the user
            var result = await _userManager.CreateAsync(model, password);
            return CheckResultForErrors(result);
        }

        /// <summary>
        /// Updates an user entity
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> UpdateUserData(TEMSUser model)
        {
            var result = await _userManager.UpdateAsync(model);
            return CheckResultForErrors(result);
        }

        /// <summary>
        /// Sets a new password for the user
        /// </summary>
        /// <param name="model"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> ResetPassword(TEMSUser model, string password)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(model);
            var result = await _userManager.ResetPasswordAsync(model, token, password);
            return CheckResultForErrors(result);
        }

        /// <summary>
        /// Sets user's roles' claims to user and appends new claims (if specified)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="newClaims">mark as null if there aren't any additional claims</param>
        /// <returns></returns>
        public async Task<string> SetClaims(TEMSUser model, List<string> newClaims)
        {
            // Getting all of the claims (Claims that come from model's role + 
            // claims indicated here)

            if (newClaims == null)
                newClaims = new();

            List<Claim> oldClaims = (List<Claim>)await _userManager.GetClaimsAsync(model);
            
            if (newClaims == oldClaims.Select(q => q.Type))
                return null;

            List<string> userRoles = (await _userManager.GetRolesAsync(model)).ToList();
            foreach (string role in userRoles)
            {
                List<string> roleClaims = (await _roleManager
                    .GetClaimsAsync(await _roleManager.FindByNameAsync(role)))
                    .Select(q => q.Type)
                    .ToList();
                newClaims.Union(roleClaims ?? new List<string>());
            }

            // Validating claims
            foreach (string claim in newClaims)
                if (!await _unitOfWork.Privileges.isExists(q => q.Identifier == claim))
                    return "One or more invalid claims provided";

            // Removing old user claims
            if (oldClaims != null)
                await _userManager.RemoveClaimsAsync(model, oldClaims);

            // Setting new claims
            List<Claim> userClaims = newClaims.Select(q => new Claim(q, "ye")).ToList();
            var result = CheckResultForErrors(await _userManager.AddClaimsAsync(model, userClaims));

            if (result != null)
                return result;

            // Claims = changed, user token = blacklisted
            await _tokenValidator.BlacklistUserToken(model.Id);
            return null;
        }

        public async Task<TEMSUser> GetUserByUsername(string username)
        {
            return await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
        }

        public async Task<List<Claim>> GetUserClaims(TEMSUser user)
        {
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
                claims.Add(new Claim(claim.Type, claim.Value));

            return claims;
        }

        public async Task<SecurityTokenDescriptor> BuildTokenDescriptor(TEMSUser user)
        {
            List<Claim> claims = await GetUserClaims(user);

            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret));
            string algorithm = SecurityAlgorithms.HmacSha256Signature;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(_appSettings.JWT_ExpireDays),
                SigningCredentials = new SigningCredentials(key, algorithm)
            };

            return tokenDescriptor;
        }

        public async Task<string> BuildTokenString(TEMSUser user)
        {
            var tokenDescriptor = await BuildTokenDescriptor(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        private string CheckResultForErrors(IdentityResult result)
        {
            if (result.Errors.Count() > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var error in result.Errors)
                    stringBuilder.Append(error.Description + "\n");

                return stringBuilder.ToString();
            }
            return null;
        }
    }
}
