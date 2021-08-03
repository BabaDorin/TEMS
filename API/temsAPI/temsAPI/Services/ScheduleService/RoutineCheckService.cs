using System.Collections.Generic;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Managers;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services.Actions;
using temsAPI.Services.JWT;
using temsAPI.Services.ScheduleService.Actions;

namespace temsAPI.Services
{
    public class RoutineCheckService
    {
        List<IScheduledAction> scheduledActions;

        public RoutineCheckService(
            ArchieveManager archieveManager,
            TokenValidatorService tokenValidatorService,
            IUnitOfWork unitOfWork)
        {
            scheduledActions = new()
            {
                new ArchiveCleaner(archieveManager),
                new TokenCleaner(tokenValidatorService),
                new ReportSanitarizer(unitOfWork),
                new BugReportAttachmentCleaner(unitOfWork, new BugReportAttachmentFileHandler())
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
