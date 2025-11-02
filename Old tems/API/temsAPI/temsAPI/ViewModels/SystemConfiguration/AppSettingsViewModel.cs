using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using temsAPI.System_Files;

namespace temsAPI.ViewModels.SystemConfiguration
{
    public class AppSettingsViewModel
    {
        public int RoutineCheckInterval { get; set; }
        public int ArchiveInterval { get; set; }
        public int LibraryAllocatedStorageSpace { get; set; }
        public string LibraryGuestPassword { get; set; }
        public bool AllowGuestsToCreateTickets { get; set; }

        public static AppSettingsViewModel FromModel(AppSettings model)
        {
            return new AppSettingsViewModel()
            {
                RoutineCheckInterval = model.RoutineCheckIntervalHr,
                ArchiveInterval = model.ArchiveIntervalHr,
                AllowGuestsToCreateTickets = model.AllowGuestsToCreateTickets,
                LibraryAllocatedStorageSpace = model.LibraryAllocatedStorageSpaceGb,
                LibraryGuestPassword = model.LibraryGuestPassword
            };
        }
    }
}
