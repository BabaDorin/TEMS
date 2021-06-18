using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.ViewModels.IdentityViewModels
{
    public class ViewUserSimplifiedViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        public static ViewUserSimplifiedViewModel FromModel(TEMSUser user, UserManager<TEMSUser> userManager)
        {
            return new ViewUserSimplifiedViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Roles = userManager.GetRolesAsync(user).Result.ToList()
            };
        }
    }
}
