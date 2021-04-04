using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.ViewModels.IdentityViewModels
{
    public class ChangeEmailPreferencesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool GetNotifications { get; set; }

        /// <summary>
        /// Validates and instance of ChangeEmailPreferencesViewModel. Returns 
        /// null if everything is ok, otherwise returns the error message.
        /// </summary>
        /// <returns></returns>
        public string Validate()
        {
            if (Email == null || Email.Length == 0)
                return "Please, provide a value for the email address";

            if (!IsValidEmail())
                return "The value provided does not seem like an email address.";

            return null;
        }

        bool IsValidEmail()
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(Email);
                return addr.Address == Email;
            }
            catch
            {
                return false;
            }
        }
    }
}
