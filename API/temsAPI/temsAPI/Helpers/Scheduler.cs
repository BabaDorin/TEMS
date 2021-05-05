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

        /// <summary>
        /// Completely wipes data about items that were archieved for more than 30 days
        /// </summary>
        private void ClearItemsArchievedForMoreThan30Days()
        {
            // Will be implemented after adding properly delete behaviour for each entity
        }

        /// <summary>
        /// Removes files that do not have any reports associated and vice versa.
        /// Also checks if there are not any foreign files inside the reports folder
        /// </summary>
        private void SanitarizeReports()
        {

        }
    }
}
