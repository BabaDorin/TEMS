﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Services;
using temsAPI.Services.Notification;

namespace temsAPI.Controllers.NotificationControllers
{
    public class NotificationController : TEMSController
    {
        private NotificationService _notificationService;
        private NotificationManager _notificationManager;
        private IdentityService _identityService;

        public NotificationController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            NotificationService notificationService,
            NotificationManager notificationManager,
            IdentityService identityService) : base(mapper, unitOfWork, userManager)
        {
            _notificationService = notificationService;
            _notificationManager = notificationManager;
            _identityService = identityService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("notification/getlastnotifications/{take?}")]
        public async Task<JsonResult> GetLastNotifications(int take = 7)
        {
            try
            {
                var user = await _identityService.GetCurrentUserAsync();
                var notifications = await _notificationManager.GetLastNotifications(user, 0, take);

                return Json(notifications);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching user notifications", ResponseStatus.Fail);
            }
        }

        [Authorize]
        [HttpGet("notification/getAll/{skip?}/{take?}")]
        public async Task<JsonResult> GetAll(int skip = 0, int take = int.MaxValue)
        {
            try
            {
                var user = await _identityService.GetCurrentUserAsync();
                var notification = await _notificationManager.GetLastNotifications(user, skip, take);

                return Json(notification);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching notifications", ResponseStatus.Fail);
            }
        }
    }
}
