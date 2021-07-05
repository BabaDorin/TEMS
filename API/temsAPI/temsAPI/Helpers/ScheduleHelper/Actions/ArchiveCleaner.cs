using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Managers;

namespace temsAPI.Helpers.ScheduleHelper.Actions
{
    public class ArchiveCleaner : IScheduledAction
    {
        private ArchieveManager _archiveManager;
        public ArchiveCleaner(ArchieveManager archiveManager)
        {
            _archiveManager = archiveManager;
        }

        public async Task Start()
        {
            await _archiveManager.RemoveOverArchivedItems();
        }
    }
}
