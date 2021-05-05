using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using temsAPI.Contracts;
using temsAPI.Helpers.ScheduleHelper.Actions;
using temsAPI.Helpers.StaticFileHelpers;

namespace temsAPI.Helpers
{
    public class Scheduler
    {
        private Timer timer;
        IUnitOfWork _unitOfWork;
        List<IScheduledAction> scheduledActions;

        public Scheduler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            timer = new Timer();
            timer.Interval = 2 * 86400 * 1000; // Fires ones every 2 days
            timer.Elapsed += Timer_Elapsed;

            scheduledActions = new()
            {
                new ArchiveCleaner(_unitOfWork)
            };
        }

        public void Start()
        {
            timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach(var action in scheduledActions)
            {
                await action.Start();
            }
        }
    }
}
