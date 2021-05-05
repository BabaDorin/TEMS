using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Helpers.ScheduleHelper.Actions
{
    public class ArchiveCleaner : IScheduledAction
    {
        IUnitOfWork _unitOfWork;
        public ArchiveCleaner(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Completely wipes data about items that were archieved for more than 30 days
        /// </summary>
        public async Task Start()
        {
            throw new NotImplementedException();
        }
    }
}
