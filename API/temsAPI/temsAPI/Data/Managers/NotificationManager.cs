using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Services;
using temsAPI.ViewModels.Notification;

namespace temsAPI.Data.Managers
{
    public class NotificationManager : EntityManager
    {
        private IdentityService _identityService;

        public NotificationManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            IdentityService identityService) : base(unitOfWork, user)
        {
            _identityService = identityService;
        }

        public async Task<List<NotificationViewModel>> GetLastNotifications(TEMSUser user, int skip = 0, int take = 7)
        {
            var notifications = (await GetLastCommonNotifications(user, take))
                .Concat(await GetLastUserNotifications(user, take))
                .OrderBy(q => q.DateCreated)
                .Take(take)
                .ToList();

            return notifications ?? new List<NotificationViewModel>();
        }

        public async Task<List<NotificationViewModel>> GetLastCommonNotifications(TEMSUser user, int take = 7)
        {
            var notifications = (await _unitOfWork.CommonNotifications
                .FindAll<NotificationViewModel>(
                    include: q => q.Include(q => q.UserCommonNotifications),
                    where: q => q.UserCommonNotifications.Select(q => q.User).Contains(user),
                    orderBy: q => q.OrderByDescending(q => q.DateCreated),
                    take: take,
                    select: q => NotificationViewModel.FromModel(q, user)
                )).ToList();

            return notifications ?? new List<NotificationViewModel>();
        }

        public async Task<List<NotificationViewModel>> GetLastUserNotifications(TEMSUser user, int take = 7)
        {
            var notifications = (await _unitOfWork.UserNotifications
                .FindAll<NotificationViewModel>(
                    where: q => q.UserID == user.Id,
                    orderBy: q => q.OrderByDescending(q => q.DateCreated),
                    take: take,
                    select: q => NotificationViewModel.FromModel(q, user)
                )).ToList();

            return notifications ?? new List<NotificationViewModel>();
        }

        public async Task<string> MarkAsSeen(List<string> notificationIds)
        {
            var user = await _identityService.GetCurrentUserAsync();

            foreach(string notiId in notificationIds)
            {
                // Search for the notification within 2 tables :(  
                INotification notification = (await _unitOfWork.UserNotifications
                    .Find<UserNotification>(q => q.Id == notiId))
                    .FirstOrDefault();
                
                if(notification == null)
                    notification = (await _unitOfWork.CommonNotifications
                    .Find<CommonNotification>(
                        where: q => q.Id == notiId,
                        include: q => q.Include(q => q.UserCommonNotifications)))
                    .FirstOrDefault();

                if (notification == null)
                    return "Invalid notification Id provided";

                notification.MarkSeen(user);
                await _unitOfWork.Save();
            }

            return null;
        }
    }
}
