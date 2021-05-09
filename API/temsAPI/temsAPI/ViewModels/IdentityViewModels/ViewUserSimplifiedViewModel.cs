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
        public string Email { get; set; }
        public string Roles { get; set; }

        public static ViewUserSimplifiedViewModel FromModel(TEMSUser user)
        {
            return new ViewUserSimplifiedViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
            };
        }
    }
}
