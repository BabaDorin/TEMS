using System.Collections.Generic;
using System.Threading.Tasks;
using temsAPI.Data.Managers;
using temsAPI.Helpers.ScheduleHelper.Actions;

namespace temsAPI.Services
{
    public class RoutineCheckService
    {
        List<IScheduledAction> scheduledActions;
        private ArchieveManager _archieveManager;

        public RoutineCheckService(
            ArchieveManager archieveManager)
        {
            _archieveManager = archieveManager;

            scheduledActions = new()
            {
                new ArchiveCleaner(_archieveManager)
            };
        }

        public async Task RoutineCheck()
        {
            foreach (var action in scheduledActions)
            {
                await action.Start();
            }
        }
    }
}
