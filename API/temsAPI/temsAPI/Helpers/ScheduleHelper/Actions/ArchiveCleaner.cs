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
        IUnitOfWork _unitOfWork;
        public ArchiveCleaner(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Start()
        {
            //Will be implemented after a major testion session on delete behaviour
            //await _unitOfWork.FindAndRemoveOverArchivedItems();
        }
    }
}
