using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels.Notification;

namespace temsAPI.Data.Managers
{
    public class NotificationManager : EntityManager
    {
        public NotificationManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
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
                    include: q => q.Include(q => q.SendTo),
                    where: q => q.SendTo.Contains(user),
                    orderBy: q => q.OrderByDescending(q => q.DateCreated),
                    take: take,
                    select: q => NotificationViewModel.FromModel(q)
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
                    select: q => NotificationViewModel.FromModel(q)
                )).ToList();

            return notifications ?? new List<NotificationViewModel>();
        }
    }
}
