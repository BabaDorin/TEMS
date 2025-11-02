using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Services;
using temsAPI.System_Files.Exceptions;

namespace temsAPI.Controllers.NotificationControllers
{
    public class NotificationController : TEMSController
    {
        readonly NotificationManager _notificationManager;
        readonly IdentityService _identityService;

        public NotificationController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            NotificationManager notificationManager,
            IdentityService identityService,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _notificationManager = notificationManager;
            _identityService = identityService;
        }

        [Authorize]
        [HttpGet("notification/GetLastNotifications/{take?}")]
        [DefaultExceptionHandler("An error occured while fetching user notifications")]
        public async Task<IActionResult> GetLastNotifications(int take = 7)
        {
            var user = await _identityService.GetCurrentUserAsync();
            var notifications = await _notificationManager.GetLastNotifications(user, 0, take);

            return Ok(notifications);
        }

        [Authorize]
        [HttpGet("notification/GetAllNotifications/{skip?}/{take?}")]
        [DefaultExceptionHandler("An error occured while fetching notifications")]
        public async Task<IActionResult> GetAllNotifications(int skip = 0, int take = int.MaxValue)
        {
            var user = await _identityService.GetCurrentUserAsync();
            var notification = await _notificationManager.GetLastNotifications(user, skip, take);

            return Ok(notification);
        }

        [Authorize]
        [HttpGet("notification/MarkAsSeen")]
        [DefaultExceptionHandler("An error occured while marking notifications as seen")]
        public async Task<IActionResult> MarkAsSeen(List<string> notificationIds)
        {
            if (notificationIds == null)
                return ReturnResponse("No notifications provided", ResponseStatus.Neutral);

            var result = await _notificationManager.MarkAsSeen(notificationIds);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [Authorize]
        [HttpDelete("notification/Remove/{notificationId}")]
        [DefaultExceptionHandler("An error occured while removing the specified notification")]
        public async Task<IActionResult> Remove(string notificationId)
        {
            var notification = await _notificationManager.GetNotificationById(notificationId);
            if (notification == null)
                return ReturnResponse("Invalid id provided", ResponseStatus.Neutral);

            string result = await _notificationManager.RemoveNotification(notification);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }
    }
}
