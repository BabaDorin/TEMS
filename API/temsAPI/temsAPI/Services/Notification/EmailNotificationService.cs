using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.ViewModels.Email;

namespace temsAPI.Services.Notification
{
    public class EmailNotificationService : INotificationService
    {
        private EmailService _emailService;
        public EmailNotificationService(EmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<string> SendNotification(INotification notification)
        {
            SendEmailViewModel emailViewModel = new()
            {
                From = "TEMS CIH Cahul",
                Addressees = notification.GetUsers().Select(q => q.Id).ToList(),
                Subject = notification.Title,
                Text = notification.Message
            };

            await _emailService.SendEmailToUsers(emailViewModel);
            return null;
        }
    }
}
