using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            IdentityService identityService,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _notificationService = notificationService;
            _notificationManager = notificationManager;
            _identityService = identityService;
        }

        [Authorize]
        [HttpGet("notification/GetLastNotifications/{take?}")]
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
                LogException(ex);
                return ReturnResponse("An error occured while fetching user notifications", ResponseStatus.Fail);
            }
        }

        [Authorize]
        [HttpGet("notification/GetAllNotifications/{skip?}/{take?}")]
        public async Task<JsonResult> GetAllNotifications(int skip = 0, int take = int.MaxValue)
        {
            try
            {
                var user = await _identityService.GetCurrentUserAsync();
                var notification = await _notificationManager.GetLastNotifications(user, skip, take);

                return Json(notification);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching notifications", ResponseStatus.Fail);
            }
        }

        [Authorize]
        [HttpGet("notification/MarkAsSeen")]
        public async Task<JsonResult> MarkAsSeen(List<string> notificationIds)
        {
            try
            {
                if (notificationIds == null)
                    return ReturnResponse("No notifications provided", ResponseStatus.Fail);

                var result = await _notificationManager.MarkAsSeen(notificationIds);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while marking notifications as seen", ResponseStatus.Fail);
            }
        }

        [Authorize]
        [HttpDelete("notification/Remove/{notificationId}")]
        public async Task<JsonResult> Remove(string notificationId)
        {
            try
            {
                var notification = await _notificationManager.GetNotificationById(notificationId);
                if (notification == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                string result = await _notificationManager.RemoveNotification(notification);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the specified notification", ResponseStatus.Fail);
            }
        }
    }
}
