using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
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
        public ClaimsPrincipal User => _user;

        public IdentityService(
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<AppSettings> appSettings,
            ClaimsPrincipal user)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _appSettings = appSettings.Value;
            _user = user;
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
        public async Task<string> AssignRoles(TEMSUser model, List<Option> roles)
        {
            // Removing current roles (if those exists)
            var result = await _userManager.RemoveFromRolesAsync(
                model, await _userManager.GetRolesAsync(model));

            if (result.Errors.Count() > 0)
                return CheckResultForErrors(result);

            result = await _userManager.AddToRoleAsync(model, "User");

            if (roles.Count > 0)
            {
                List<string> roleNames = roles.Select(q => q.Label).ToList();

                // This lines are here to protect agains invalid role names being sent
                List<string> actualRoles = _roleManager
                    .Roles
                    .Where(q => roleNames.Contains(q.Name) && q.Name != "User")
                    .Select(q => q.Name)
                    .ToList();

                result = await _userManager.AddToRolesAsync(model, actualRoles);
            }

            if (result.Errors.Count() > 0)
                return CheckResultForErrors(result);

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
        /// <param name="claims">mark as null if there aren't any additional claims</param>
        /// <returns></returns>
        public async Task<string> SetClaims(TEMSUser model, List<string> claims)
        {
            // Getting all of the claims (Claims that come from model's role + 
            // claims indicated here)

            List<string> allClaims = claims ?? new List<string>();

            List<string> userRoles = (await _userManager.GetRolesAsync(model)).ToList();
            foreach (string role in userRoles)
            {
                List<string> roleClaims = (await _roleManager
                    .GetClaimsAsync(await _roleManager.FindByNameAsync(role)))
                    .Select(q => q.Type)
                    .ToList();
                allClaims.Union(roleClaims ?? new List<string>());
            }

            // Validating claims
            foreach (string claim in allClaims)
                if (!await _unitOfWork.Privileges.isExists(q => q.Identifier == claim))
                    return "One or more invalid claims provided";

            // Removing old user claims
            List<Claim> oldClaims = (List<Claim>)await _userManager.GetClaimsAsync(model);
            if (oldClaims != null)
                await _userManager.RemoveClaimsAsync(model, oldClaims);

            // Setting new claims
            List<Claim> userClaims = allClaims.Select(q => new Claim(q, "ye")).ToList();
            var result = await _userManager.AddClaimsAsync(model, userClaims);

            return CheckResultForErrors(result);
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
                userClaims = userClaims.Union(await _userManager.GetClaimsAsync(await _userManager.FindByNameAsync(role)))
                    .Select(q => new Claim(q.Type, q.Value))
                    .ToList();

            foreach (var claim in userClaims)
                claims.Add(new Claim(claim.Type, claim.Value));

            return claims;
        }

        public async Task<SecurityTokenDescriptor> BuildTokenDescriptor(TEMSUser user)
        {
            List<Claim> claims = await GetUserClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            _appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
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
