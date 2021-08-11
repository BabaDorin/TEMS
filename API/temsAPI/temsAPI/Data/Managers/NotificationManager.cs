using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Factories.Email;
using temsAPI.Data.Factories.Notification;
using temsAPI.Helpers.NotificationProviders;
using temsAPI.Helpers.ReusableSnippets;
using temsAPI.Services;
using temsAPI.ViewModels.Email;
using temsAPI.ViewModels.Notification;

namespace temsAPI.Data.Managers
{
    public class NotificationManager : EntityManager
    {
        IdentityService _identityService;
        UserManager<TEMSUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        SystemConfigurationService _configService;
        EmailService _emailService;

        public NotificationManager(
            IUnitOfWork unitOfWork, 
            ClaimsPrincipal user,
            IdentityService identityService,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SystemConfigurationService configService,
            EmailService emailService) : base(unitOfWork, user)
        {
            _identityService = identityService;
            _userManager = userManager;
            _roleManager = roleManager;
            _configService = configService;
            _emailService = emailService;
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
            var assignees = ticket.Assignees?
                .Where(q => !String.IsNullOrEmpty(q.Email))
                .ToList();

            if (assignees != null && assignees.Count > 0)
                await CreateAndSendNotification(new TicketAssignedNotificationBuilder(ticket, assignees).Create());

            // Notify technicians
            var technicians = (await _userManager.GetUsersInRoleAsync("technician"))
                .Where(q => q.GetEmailNotifications && !String.IsNullOrEmpty(q.Email))
                .Except(assignees)
                .ToList();

            if (technicians != null && technicians.Count > 0)
                await CreateAndSendNotification(new TicketCreatedNotificationBuilder(ticket, technicians).Create());

            // BEFREE -> Notify supervisories
        }

        /// <summary>
        /// Sends an email with attachments to e-mail addresses specified in appsettings.json
        /// and creates a notification addressed to system administrators
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public async Task<string> NotifyBugReport(BugReport report)
        {
            await NotifyBugReportAppNotifications(report);
            string emailNotifyingResult = await NotifyBugReportEmail(report);
            return emailNotifyingResult;
        }

        /// <summary>
        /// Notifies about bug report via e-mail
        /// </summary>
        /// <param name="report"></param>
        /// <returns>Null if everything is ok, otherwise - an error message</returns>
        private async Task<string> NotifyBugReportEmail(BugReport report)
        {
            var emailBuilder = new BugReportEmailBuilder(report, _configService.AppSettings);
            var emailData = await emailBuilder.BuildEmailModel();
            if (emailData.Recipients.IsNullOrEmpty())
                return "Your report has been saved, but no e-mail was sent because there aren't any recipients specified";

            _ = Task.Factory.StartNew(() => new EmailNotificationProvider(_emailService, emailData).SendNotification());
            
            return null;
        }

        private async Task NotifyBugReportAppNotifications(BugReport report)
        {
            // Notify administrators
            var administrators = await _userManager.GetUsersInRoleAsync("Administrator");

            if (administrators != null && administrators.Count > 0)
            {
                var notification = (new BugReportCreatedNotificationBuilder(report, administrators).Create());
                await CreateCommonNotification(notification);
            }
        }

        /// <summary>
        /// Saved the notification and notifies according to the data being provided by the notification itself
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        private async Task CreateAndSendNotification(INotification notification)
        {
            if(notification is CommonNotification)
                await CreateCommonNotification(notification as CommonNotification);
            else
                await CreateUserNotification(notification as UserNotification);

            var emailData = EmailData.FromNotification(notification);
            _ = Task.Factory.StartNew(() => new EmailNotificationProvider(_emailService, emailData).SendNotification());
        }
    }
}
