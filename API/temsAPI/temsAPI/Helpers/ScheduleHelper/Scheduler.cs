using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using temsAPI.Contracts;
using temsAPI.Helpers.ScheduleHelper.Actions;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services;

namespace temsAPI.Helpers
{
    public class Scheduler
    {
        readonly Timer timer;
        readonly SystemConfigurationService _systemConfigurationService;
        readonly IServiceScopeFactory _serviceScopeFactory;

        public Scheduler(
            SystemConfigurationService systemConfigurationService,
            IServiceScopeFactory serviceScopeFactory)
        {
            _systemConfigurationService = systemConfigurationService;
            _serviceScopeFactory = serviceScopeFactory;

            timer = new Timer
            {
                Interval = GetTimerInterval(),
            };
            timer.Elapsed += Timer_Elapsed;

            _systemConfigurationService.RoutineNotifier.RoutineCheckInterval_Changed += Interval_Changed;
        }

        public void Start()
        {
            timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var routineCheckService = scope.ServiceProvider.GetService<RoutineCheckService>();
            await routineCheckService.RoutineCheck();
        }

        private void Interval_Changed(object sender, EventArgs e)
        {
            bool enabled = timer.Enabled;
            if (enabled)
                timer.Stop();

            timer.Interval = GetTimerInterval();
            if (enabled)
                timer.Start();
        }

        private int GetTimerInterval()
        {
            return _systemConfigurationService.AppSettings.RoutineCheckIntervalHr * 60 * 60 * 1000;
        }
    }
}
