using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace temsAPI.Helpers
{
    public class Scheduler
    {
        private Timer timer;

        public Scheduler()
        {
            timer = new Timer();
            timer.Interval = 2 * 86400 * 1000; // Fires ones every 2 days
            timer.Elapsed += Timer_Elapsed;
        }

        public void Start()
        {
            ClearItemsArchievedForMoreThan30Days();
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ClearItemsArchievedForMoreThan30Days();
        }

        private void ClearItemsArchievedForMoreThan30Days()
        {
            // Will be implemented after adding properly delete behaviour for each entity
        }
    }
}
