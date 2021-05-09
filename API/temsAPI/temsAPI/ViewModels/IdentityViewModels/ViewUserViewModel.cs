using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.ViewModels.IdentityViewModels
{
    public class ViewUserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public Option Personnel { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Claims { get; set; }

        public static ViewUserViewModel FromModel(TEMSUser user, UserManager<TEMSUser> userManager)
        {
            return new ViewUserViewModel
            {
                Id = user.Id,
                Claims = userManager.GetClaimsAsync(user)
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
                Roles = userManager.GetRolesAsync(user).Result?.ToList(),
            };
        }
    }
}
