using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers.ScheduleHelper.Actions
{
    interface IScheduledAction
    {
        Task Start();
    }
}
