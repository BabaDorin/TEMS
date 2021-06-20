using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.System_Files;

namespace temsAPI.Services
{
    public class SystemConfigurationService
    {
        public AppSettings AppSettings { get; private set; }
        
        public SystemConfigurationService(IOptions<AppSettings> appSettingsOptions)
        {
            AppSettings = appSettingsOptions.Value;
        }

        public string SetEmailSender(string address, string password)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password))
                return "Invalid e-mail address or password";

            AppSettings.Email.EmailSenderAddress = address;
            AppSettings.Email.EmailSenderAddressPassword = password;
            
            return null;
        }

        public void SetClientUrl(string clientUrl)
        {
            AppSettings.Client_Url = clientUrl;
        }

        public void SetJWTSecret(string newJwtSecret)
        {
            AppSettings.JWT_Secret = newJwtSecret;
        }

        public string SetRoutineCheckInterval(int hours)
        {
            if (hours <= 0)
                return "Invalid value provided for hours.";

            AppSettings.RoutineCheckIntervalHr = hours;
            return null;
        }

        public string SetArchieveIntervalHr(int hours)
        {
            if (hours <= 0)
                return "Invalid value provided for hours.";

            AppSettings.ArchiveIntervalHr = hours;
            return null;
        }

        public string SetLibraryAllocatedStorageSpace(int gb)
        {
            if (gb <= 0)
                return "Invalid value provided for gb.";

            var libraryFileHanler = StaticFileHelper.GetFileHandler(StaticFileHandlers.LibraryItem);
            if (gb <= StaticFileHelper.DirSizeBytes(new System.IO.DirectoryInfo(libraryFileHanler.FolderPath)))
                return "The indicated value is less than the storage space which is already used";

            AppSettings.ArchiveIntervalHr = gb;
            return null;
        }

        public void SetLibraryGuestPass(string newPass)
        {
            AppSettings.LibraryGuestPassword = newPass;
        }

        public string SetGeneratedReportsHistoryLength(int length)
        {
            if (length <= 0)
                return "Invalid value provided for length";

            AppSettings.GeneratedReportsHistoryLength = length;
            return null;
        }

        public void AllowGuestsToCreateTickets()
        {
            AppSettings.AllowGuestsToCreateTickets = true;
        }

        public void ForbidGuestsToCreateTickets()
        {
            AppSettings.AllowGuestsToCreateTickets = false;
        }
    }
}
