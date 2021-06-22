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
        private Timer timer;
        IUnitOfWork _unitOfWork;
        SystemConfigurationService _systemConfigurationService;
        RoutineCheckService _routineCheckService;

        public Scheduler(IUnitOfWork unitOfWork, SystemConfigurationService systemConfigurationService, RoutineCheckService routineCheckService)
        {
            _unitOfWork = unitOfWork;
            _systemConfigurationService = systemConfigurationService;
            _routineCheckService = routineCheckService;

            timer = new Timer();
            timer.Interval = _systemConfigurationService.AppSettings.RoutineCheckIntervalHr * 60 * 60 * 1000;
            timer.Elapsed += Timer_Elapsed;
        }

        public void Start()
        {
            timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await _routineCheckService.RoutineCheck();
        }
    }
}
