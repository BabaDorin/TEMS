using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.ViewModels.Profile
{
    public class ViewProfileViewModel
    {
        public string Id { get; set; }
        public Option Personnel { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsArchieved { get; set; }
        public DateTime DateArchieved { get; set; }
        public DateTime DateRegistered { get; set; }
    }
}
