﻿using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.System_Files;
using temsAPI.System_Files.TEMSFileLogger;

namespace temsAPI.Services
{
    public class SystemConfigurationService
    {
        public AppSettings AppSettings { get; private set; } // this instance is used by the system
        private IWritableOptions<AppSettings> _appSettingsOptions; // this is used for file interaction
        private FileLoggerSettings _loggerSettings;

        public SystemConfigurationService(
            IWritableOptions<AppSettings> appSettingsOptions,
            IOptions<FileLoggerSettings> loggerSettings)
        {
            AppSettings = appSettingsOptions.Value;
            _appSettingsOptions = appSettingsOptions;
            _loggerSettings = loggerSettings.Value;
        }

        public string SetEmailSender(string address, string password)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password))
                return "Invalid e-mail address or password";

            AppSettings.Email.EmailSenderAddress = address;
            AppSettings.Email.EmailSenderAddressPassword = password;

            this._appSettingsOptions.Update(op =>
            {
                op.Email.EmailSenderAddress = address;
                op.Email.EmailSenderAddressPassword = password;
            });
            return null;
        }

        public void SetClientUrl(string clientUrl)
        {
            AppSettings.Client_Url = clientUrl;

            this._appSettingsOptions.Update(op =>
            {
                op.Client_Url = clientUrl;
            });
        }

        public void SetJWTSecret(string newJwtSecret)
        {
            AppSettings.JWT_Secret = newJwtSecret;

            this._appSettingsOptions.Update(op =>
            {
                op.JWT_Secret = newJwtSecret;
            });
        }

        public string SetRoutineCheckInterval(int hours)
        {
            if (hours <= 0)
                return "Invalid value provided for hours.";

            AppSettings.RoutineCheckIntervalHr = hours;
            
            this._appSettingsOptions.Update(op =>
            {
                op.RoutineCheckIntervalHr = hours;
            });

            return null;
        }

        public string SetArchieveIntervalHr(int hours)
        {
            if (hours <= 0)
                return "Invalid value provided for hours.";

            AppSettings.ArchiveIntervalHr = hours;

            this._appSettingsOptions.Update(op =>
            {
                op.ArchiveIntervalHr = hours;
            });

            return null;
        }

        public string SetLibraryAllocatedStorageSpace(int gb)
        {
            if (gb <= 0)
                return "Invalid value provided for gb.";

            var libraryFileHanler = StaticFileHelper.GetFileHandler(StaticFileHandlers.LibraryItem);
            if (gb <= StaticFileHelper.DirSizeBytes(new System.IO.DirectoryInfo(libraryFileHanler.FolderPath)) / 1073741824)
                return "The indicated value is less than the storage space which is already used";

            AppSettings.LibraryAllocatedStorageSpaceGb = gb;

            this._appSettingsOptions.Update(op =>
            {
                op.LibraryAllocatedStorageSpaceGb = gb;
            });

            return null;
        }

        public void SetLibraryGuestPass(string newPass)
        {
            AppSettings.LibraryGuestPassword = newPass;

            this._appSettingsOptions.Update(op =>
            {
                op.LibraryGuestPassword = newPass;
            });
        }

        public string SetGeneratedReportsHistoryLength(int length)
        {
            if (length <= 0)
                return "Invalid value provided for length";

            AppSettings.GeneratedReportsHistoryLength = length;

            this._appSettingsOptions.Update(op =>
            {
                op.GeneratedReportsHistoryLength = length;
            });

            return null;
        }

        public void AllowGuestsToCreateTickets()
        {
            AppSettings.AllowGuestsToCreateTickets = true;

            this._appSettingsOptions.Update(op =>
            {
                op.AllowGuestsToCreateTickets = true;
            });
        }

        public void ForbidGuestsToCreateTickets()
        {
            AppSettings.AllowGuestsToCreateTickets = false;

            this._appSettingsOptions.Update(op =>
            {
                op.AllowGuestsToCreateTickets = false;
            });
        }

        public string GetLogsByDate(DateTime date)
        {
            string directoryPath = _loggerSettings.FolderPath;
            string filePath = directoryPath + '\\' + _loggerSettings.FilePath
                .Replace("{date}", date.ToString("yyyyMMdd"));

            if(Directory.Exists(directoryPath) && File.Exists(filePath))
                using (StreamReader sr = new StreamReader(filePath))
                    return sr.ReadToEnd();

            return null;
        }
    }
}
