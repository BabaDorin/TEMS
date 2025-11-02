using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.IdentityViewModels
{
    public class AddUserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Option Personnel { get; set; }
        public List<Option> Roles { get; set; }
        public List<string> Claims { get; set; }
    }
}
