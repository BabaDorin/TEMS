using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.IdentityViewModels
{
    public class AccountGeneralInfoViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        
        /// <summary>
        /// Validates an instance of AccountGeneralInfoViewModel. Returns null if everything
        /// is ok, otherwise returns the error message.
        /// </summary>
        /// <returns></returns>
        public string Validate()
        {
            if (Username == null || Username.Length < 4)
                return "Username should be at least 4 characters long";

            if(Username.Length > 150)
                return "It is just me or is that username looking kinda T H I C C";

            if (FullName.Length > 150)
                return "Isn't that name to big? :/";

            return null;
        }
    }
}
