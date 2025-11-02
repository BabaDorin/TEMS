using System.Threading.Tasks;
using temsAPI.Data.Managers;

namespace temsAPI.Services.Actions
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
