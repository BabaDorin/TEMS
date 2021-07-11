using System.Collections.Generic;
using System.Threading.Tasks;
using temsAPI.Data.Managers;
using temsAPI.Helpers.ScheduleHelper.Actions;
using temsAPI.Services.JWT;

namespace temsAPI.Services
{
    public class RoutineCheckService
    {
        List<IScheduledAction> scheduledActions;

        public RoutineCheckService(
            ArchieveManager archieveManager,
            TokenValidatorService tokenValidatorService)
        {
            scheduledActions = new()
            {
                new ArchiveCleaner(archieveManager),
                new TokenCleaner(tokenValidatorService)
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
