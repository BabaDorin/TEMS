using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.IdentityViewModels;
using temsAPI.ViewModels.Profile;

namespace temsAPI.Data.Managers
{
    public class TEMSUserManager
    {
        private UserManager<TEMSUser> _userManager;
        private IUnitOfWork _unitOfWork;
        private ClaimsPrincipal _user;
        private AppSettings _appSettings;
        private IdentityService _identityService;
        RoleManager<IdentityRole> _roleManager;

        public TEMSUserManager(
            UserManager<TEMSUser> userManager,
            IUnitOfWork unitOfWork,
            ClaimsPrincipal user,
            IOptions<AppSettings> appSettings,
            RoleManager<IdentityRole> roleManager,
            IdentityService identityService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _user = user;
            _appSettings = appSettings.Value;
            _roleManager = roleManager;
            _identityService = identityService;
        }

        public async Task BlacklistToken(string token)
        {
            await _unitOfWork.JWTBlacklist.Create(
                    new TemsJWT
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = token,
                        ExpirationDate = (new JWTHelper()).GetExpiryTimestamp(token)
                    });

            await _unitOfWork.Save();
        }

        public async Task<string> GenerateToken(LogInViewModel viewModel)
        {
            var user = await _identityService.GetUserByUsername(viewModel.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, viewModel.Password))
                throw new Exception("Invalid username or password");

            return await _identityService.BuildTokenString(user);
        }

        public async Task<ViewProfileViewModel> GetProfileInfo(string userId)
        {
            var profile = (await _unitOfWork.TEMSUsers
                    .Find<ViewProfileViewModel>(
                        where: q => q.Id == userId,
                        include: q => q
                        .Include(q => q.AssignedTickets)
                        .Include(q => q.ClosedTickets)
                        .Include(q => q.CreatedTickets)
                        .Include(q => q.Announcements),
                        select: q => ViewProfileViewModel.FromModel(q)
                    )).FirstOrDefault();

            profile.Roles = (await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId)))
                .ToList();

            return profile;
        }

        public async Task<string> CreateUser(AddUserViewModel viewModel)
        {
            // Username and password are mandatory
            viewModel.Username = viewModel.Username?.Trim();
            if (String.IsNullOrEmpty(viewModel.Username) || String.IsNullOrEmpty(viewModel.Password))
                return "Invalid username or password provided";

            TEMSUser model = new TEMSUser()
            {
                UserName = viewModel.Username,
                FullName =
                    String.IsNullOrEmpty(viewModel.FullName)
                    ? viewModel.Username
                    : viewModel.FullName,
                PhoneNumber = viewModel.PhoneNumber,
                Email = viewModel.Email,
                DateRegistered = DateTime.Now,
                GetEmailNotifications = (viewModel.Roles.FindIndex(q => q.Label.ToLower() == "technician" || q.Label.ToLower() == "tehnician") != -1)
                    ? true
                    : false
            };

            // Email already exists.
            if (model.Email != null)
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                    return "The provided email is already in use";

            // Creating the user
            string result = await _identityService.CreateUser(model, viewModel.Password);
            if (result != null)
                return result;

            // Assigning roles
            result = await _identityService.AssignRoles(model, viewModel.Roles);
            if (result != null)
                return result;

            // Setting claims
            result = await _identityService.SetClaims(model, viewModel.Claims);
            if (result != null)
                return result;

            // Creating Personnel-User Association
            result = await _identityService.UserPersonnelAssociation(model, viewModel.Personnel);
            if (result != null)
                return result;

            return null;
        }

        public async Task<string> UpdateUser(AddUserViewModel viewModel)
        {
            string result = null;

            // Invalid id
            if (!await _unitOfWork.TEMSUsers.isExists(q => q.Id == viewModel.Id))
                return "Invalid Id provided";

            // Invalid username
            viewModel.Username = viewModel.Username?.Trim();
            if (String.IsNullOrEmpty(viewModel.Username))
                return "Invalid username provided";

            var model = await _userManager.FindByIdAsync(viewModel.Id);

            // Password reset (If needed)
            if (viewModel.Password != null)
            {
                result = await _identityService.ResetPassword(model, viewModel.Password);
                if (result != null)
                    return result;
            }

            model.UserName = viewModel.Username;
            model.PhoneNumber = viewModel.PhoneNumber;
            model.Email = viewModel.Email;
            model.FullName = viewModel.FullName;

            // Updating properties
            result = await _identityService.UpdateUserData(model);
            if (result != null)
                return result;

            // Assigning roles
            result = await _identityService.AssignRoles(model, viewModel.Roles);
            if (result != null)
                return result;

            // Setting claims
            result = await _identityService.SetClaims(model, viewModel.Claims);
            if (result != null)
                return result;

            // Creating Personnel-Personnel Association
            result = await _identityService.UserPersonnelAssociation(model, viewModel.Personnel);
            if (result != null)
                return result;

            return null;
        }

        public async Task<List<Option>> GetAutocompleteOptions(string filter)
        {
            Expression<Func<TEMSUser, bool>> expression =
                    q => !q.IsArchieved && q.UserName != "tems@dmin";

            if (filter != null)
            {
                Expression<Func<TEMSUser, bool>> expression2 =
                    q => (q.UserName.Contains(filter) || q.FullName.Contains(filter));
                expression = ExpressionCombiner.CombineTwo(expression, expression2);
            }

            var options = (await _unitOfWork.TEMSUsers
                .FindAll<Option>(
                    where: expression,
                    take: 5,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.FullName ?? q.UserName
                    })).ToList();

            return options;
        }

        public async Task<List<ViewUserSimplifiedViewModel>> GetUsers(string role, int skip = 0, int take = int.MaxValue)
        {
            IEnumerable<TEMSUser> users;

            if(role == null)
            {
                Expression<Func<TEMSUser, bool>> expression = q => !q.IsArchieved && q.UserName != "tems@dmin";

                users = (await _unitOfWork.TEMSUsers
                .FindAll<TEMSUser>(
                    where: expression,
                    skip: skip,
                    take: take));
            }
            else
            {
                users = await GetUsersOfRole(role, skip, take);
            }

            return users.Select(q => ViewUserSimplifiedViewModel.FromModel(q, _userManager)).ToList();
        }

        public async Task<IEnumerable<TEMSUser>> GetUsersOfRole(string role, int skip = 0, int take = int.MaxValue)
        {
            return (await _userManager.GetUsersInRoleAsync(role))
                    .Where(q => !q.IsArchieved && q.UserName != "tems@dmin")
                    .Skip(skip)
                    .Take(take);
        }

        public async Task<List<Option>> GetClaims()
        {
            var claims = (await _unitOfWork.Privileges.FindAll<Option>(
                select: q => new Option
                {
                    Value = q.Identifier,
                    Label = q.Identifier,
                    Additional = q.Description
                }
                )).ToList();

            return claims;
        }

        public async Task<List<string>> GetRolesCumulativeClaims(List<string> roles)
        {
            List<string> claims = new List<string>();
            foreach (var role in roles)
            {
                var roleClaims = await _roleManager
                    .GetClaimsAsync(await _roleManager.FindByNameAsync(role));

                claims = claims.Union(roleClaims.Select(q => q.Type)).ToList();
            }

            return claims;
        }

        public async Task<List<string>> GetUserClaims(string userId)
        {
            var user = (await _unitOfWork.TEMSUsers
                .Find<TEMSUser>(q => q.Id == userId))
                .FirstOrDefault();
            if (user == null)
                throw new Exception("Invalid user Id provided");

            List<string> claims = (await _userManager
                .GetClaimsAsync(user))
                .Select(q => q.Type)
                .ToList();

            return claims;
        }

        public async Task<ViewUserViewModel> GetUser(string userId)
        {
            var user = (await _unitOfWork.TEMSUsers
                    .Find<ViewUserViewModel>(
                        where: q => q.Id == userId,
                        include: q => q
                        .Include(q => q.Personnel),
                        select: q => ViewUserViewModel.FromModel(q, _userManager)
                    )).FirstOrDefault();

            return user;
        }

        public async Task<string> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (viewModel.UserId != _identityService.GetUserId()) ;
            return "You don't have enough privilleges to change this user's password ;)";

            string validationResult = viewModel.Validate();
            if (validationResult != null)
                return validationResult;

            if (viewModel.OldPass == viewModel.NewPass)
                return "Your new password matches the old one.";

            var user = await _userManager.FindByIdAsync(viewModel.UserId);
            var result = _userManager.ChangePasswordAsync(user, viewModel.OldPass, viewModel.NewPass);

            if (result.Result.Errors.Count() > 0)
                return "The password has not been changed. Make sure the data you've provided is valid";

            return null;
        }

        public async Task<string> ChangeEmailPreferences(ChangeEmailPreferencesViewModel viewModel)
        {
            if (viewModel.UserId != _identityService.GetUserId())
                return "You don't have enough privilleges to change this user's email configuration ;)";

            string validationResult = viewModel.Validate();
            if (validationResult != null)
                return validationResult;

            var user = (await _unitOfWork.TEMSUsers
                .Find<TEMSUser>(q => q.Id == viewModel.UserId))
                .FirstOrDefault();

            // Will come back later to handle emails via usermanager and to implement
            // email confirmation.
            if (viewModel.Email != user.Email && await _userManager.FindByEmailAsync(viewModel.Email) != null)
                return "The provided email is aldreay in use.";

            user.Email = viewModel.Email;
            user.NormalizedEmail = viewModel.Email.ToUpper();
            user.GetEmailNotifications = viewModel.GetNotifications;

            await _unitOfWork.Save();

            return null;
        }

        public async Task<string> EditAccountInfo(AccountGeneralInfoViewModel viewModel)
        {
            if (viewModel.UserId != _identityService.GetUserId())
                return "You don't have enough privilleges to change this user's account info ;)";

            string validationResult = viewModel.Validate();
            if (validationResult != null)
                return validationResult;

            var user = (await _unitOfWork.TEMSUsers
                .Find<TEMSUser>(q => q.Id == viewModel.UserId))
                .FirstOrDefault();

            if (user.UserName != viewModel.Username)
            {
                var result = await _userManager.SetUserNameAsync(user, viewModel.Username);
                if (!result.Succeeded)
                    return "Either the provided username is invalid or it is already in use.";
            }

            user.FullName = viewModel.FullName;
            await _unitOfWork.Save();

            return null;
        }

        public async Task<List<Option>> GetRolesOptions()
        {
            var roles = await _roleManager
                .Roles
                .Select(q => new Option
                {
                    Value = q.Id,
                    Label = q.Name
                }).ToListAsync();

            return roles;
        }
    }
}
