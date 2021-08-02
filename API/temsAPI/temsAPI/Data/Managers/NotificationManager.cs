using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Factories.Notification;
using temsAPI.Services;
using temsAPI.Services.Notification;
using temsAPI.ViewModels.Notification;

namespace temsAPI.Data.Managers
{
    public class NotificationManager : EntityManager
    {
        private IdentityService _identityService;
        private UserManager<TEMSUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        EmailNotificationService _emailNotificationService;

        public NotificationManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            IdentityService identityService,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            EmailNotificationService emailNotificationService) : base(unitOfWork, user)
        {
            _identityService = identityService;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailNotificationService = emailNotificationService;
        }

        public async Task CreateUserNotification(UserNotification notification)
        {
            await _unitOfWork.UserNotifications.Create(notification);
            await _unitOfWork.Save();
        }

        public async Task CreateCommonNotification(CommonNotification notification)
        {
            await _unitOfWork.CommonNotifications.Create(notification);
            await _unitOfWork.Save();
        }

        public async Task RemoveUserNotification(INotification notification)
        {
            _unitOfWork.UserNotifications.Delete((UserNotification)notification);
            await _unitOfWork.Save();
        }

        public async Task RemoveCommonNotification(INotification notification)
        {
            _unitOfWork.CommonNotifications.Delete((CommonNotification)notification);
            await _unitOfWork.Save();
        }

        public async Task<string> RemoveNotification(INotification notification)
        {
            if (await _unitOfWork.UserNotifications.isExists(q => q.Id == notification.Id))
                await RemoveUserNotification(notification);
            else
                await RemoveCommonNotification(notification);

            return null;
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
        
        public async Task<INotification> GetNotificationById(string notificationId)
        {
            INotification notification = await GetUserNotificationById(notificationId);
            if (notification == null)
                notification = await GetCommonNotificationById(notificationId);

            return notification;
        }

        public async Task<UserNotification> GetUserNotificationById(string notificationId)
        {
            return (await _unitOfWork.UserNotifications
                .Find<UserNotification>(q => q.Id == notificationId))
                .FirstOrDefault();
        }
        
        public async Task<CommonNotification> GetCommonNotificationById(string notificationId)
        {
            return (await _unitOfWork.CommonNotifications
                .Find<CommonNotification>(q => q.Id == notificationId))
                .FirstOrDefault();
        }
        
        public async Task NotifyTicketCreation(Ticket ticket)
        {
            // Notify assignees
            var assigneeIds = ticket.Assignees?.Select(q => q.Id).ToList();
            if(assigneeIds != null && assigneeIds.Count > 0)
            {
                var notification = (new TicketAssignedNotificationBuilder(ticket).Create());
                await CreateCommonNotification(notification);
                await _emailNotificationService.SendNotification(notification);
            }

            // Notify technicians
            var techIds = (await _userManager.GetUsersInRoleAsync("technician"))
                ?.Where(q => q.GetEmailNotifications)
                .Select(q => q.Id)
                .Except(assigneeIds)
                .ToList();

            if(techIds != null && techIds.Count > 0)
            {
                var notification = new TicketCreatedNotificationBuilder(ticket, techIds).Create();
                await CreateCommonNotification(notification);
                await _emailNotificationService.SendNotification(notification);
            }

            // BEFREE -> Notify supervisories
        }

        public async Task NotifyBugReport(BugReport report)
        {
            //// Notify administrators
            var adminIds = (await _userManager.GetUsersInRoleAsync("Administrator"))
                .Select(q => q.Id)
                .ToList();

            adminIds.Remove(report.CreatedByID);

            if (adminIds != null && adminIds.Count > 0)
            {
                var notification = (new BugReportCreatedNotificationBuilder(report, adminIds).Create());
                await CreateCommonNotification(notification);
                await _emailNotificationService.SendNotification(notification);
            }
        }
    }
}
