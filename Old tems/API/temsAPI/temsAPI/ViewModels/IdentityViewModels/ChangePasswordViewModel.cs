using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.IdentityViewModels
{
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string ConfirmNewPass { get; set; }

        /// <summary>
        /// Verifies the validity of an instance of ChangePasswordViewModel. Returns null
        /// if everything is ok, otherwise returns the error message.
        /// </summary>
        /// <returns></returns>
        public string Validate()
        {
            if (NewPass.Length < 5)
                return "The password should be at least 5 characters long.";

            if (NewPass.Length > 150)
                return "Isn't that too much for a passowrd? :/";

            if (NewPass != ConfirmNewPass)
                return "Passwords do not match";

            return null;
        }
    }
}
