using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;

namespace temsAPI.Helpers
{
    public class UserHelper
    {
        private IUnitOfWork _unitOfWork;
        private UserManager<TEMSUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public UserHelper(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
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
            //await _userManager.RemovePasswordAsync(model);
            //var result = await _userManager.AddPasswordAsync(model, password);
            var token = await _userManager.GeneratePasswordResetTokenAsync(model);
            var result = await _userManager.ResetPasswordAsync(model, token, password);
            return CheckResultForErrors(result);
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
