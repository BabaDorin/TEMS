using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Services.SystemConfiguration
{
    public class RoutineCheckNotifier
    {
        public delegate void RoutineCheckInterval_ChangedHandler(object sender, EventArgs e);
        public event RoutineCheckInterval_ChangedHandler RoutineCheckInterval_Changed;

        public void RoutineCheckIntervalChanged()
        {
            if (RoutineCheckInterval_Changed != null)
                RoutineCheckInterval_Changed(this, EventArgs.Empty);
        }
    }
}
