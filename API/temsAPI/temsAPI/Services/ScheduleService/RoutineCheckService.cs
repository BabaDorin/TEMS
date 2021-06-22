using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Managers;
using temsAPI.Helpers.ScheduleHelper.Actions;

namespace temsAPI.Services
{
    public class RoutineCheckService
    {
        ArchieveManager _archieveManager;
        List<IScheduledAction> scheduledActions;
        IUnitOfWork _unitOfWork;

        public RoutineCheckService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            scheduledActions = new()
            {
                new ArchiveCleaner(unitOfWork)
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
