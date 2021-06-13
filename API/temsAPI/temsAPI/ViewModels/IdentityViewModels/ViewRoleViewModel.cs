using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.IdentityViewModels
{
    public class ViewRoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Claims { get; set; }

        public static ViewRoleViewModel FromModel(IdentityRole role, RoleManager<IdentityRole> roleManager)
        {
            return new ViewRoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                Claims = roleManager.GetClaimsAsync(role).Result.Select(q => q.Type)
                    .ToList()
            };
        }
    }
}
