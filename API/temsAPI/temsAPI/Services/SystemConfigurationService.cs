using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.System_Files;

namespace temsAPI.Services
{
    public class SystemConfigurationService
    {
        private AppSettings _appSettings;

        public SystemConfigurationService(IOptions<AppSettings> appSettingsOptions)
        {
            _appSettings = appSettingsOptions.Value;
        }

        public string SetEmailSender(string address, string password)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password))
                return "Invalid e-mail address or password";

            _appSettings.Email.EmailSenderAddress = address;
            _appSettings.Email.EmailSenderAddressPassword = password;
            
            return null;
        }

        public void SetClientUrl(string clientUrl)
        {
            _appSettings.Client_Url = clientUrl;
        }

        public void SetJWTSecret(string newJwtSecret)
        {
            _appSettings.JWT_Secret = newJwtSecret;
        }

        public string SetRoutineCheckInterval(int hours)
        {
            if (hours <= 0)
                return "Invalid value provided for hours.";

            _appSettings.RoutineCheckIntervalHr = hours;
            return null;
        }

        public string SetArchieveIntervalHr(int hours)
        {
            if (hours <= 0)
                return "Invalid value provided for hours.";

            _appSettings.ArchiveIntervalHr = hours;
            return null;
        }

        public string SetLibraryAllocatedStorageSpace(int gb)
        {
            if (gb <= 0)
                return "Invalid value provided for gb.";

            _appSettings.ArchiveIntervalHr = gb;
            return null;
        }

        public void SetLibraryGuestPass(string newPass)
        {
            _appSettings.LibraryGuestPass = newPass;
        }

        public string SetGeneratedReportsHistoryLength(int length)
        {
            if (length <= 0)
                return "Invalid value provided for length";

            _appSettings.GeneratedReportsHistoryLength = length;
            return null;
        }

        public void AllowGuestsToCreateTickets()
        {
            _appSettings.AllowGuestsToCreateTickets = true;
        }

        public void ForbidGuestsToCreateTickets()
        {
            _appSettings.AllowGuestsToCreateTickets = false;
        }
    }
}
