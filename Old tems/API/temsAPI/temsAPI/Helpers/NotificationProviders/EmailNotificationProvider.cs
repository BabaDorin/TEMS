using System.Threading.Tasks;
using temsAPI.Services;
using temsAPI.ViewModels.Email;

namespace temsAPI.Helpers.NotificationProviders
{
    public class EmailNotificationProvider : INotificationProvider
    {
        private EmailService _emailService;
        private EmailData _emailViewModel;

        public EmailNotificationProvider(EmailService emailService, EmailData emailViewModel)
        {
            _emailService = emailService;
            _emailViewModel = emailViewModel;
        }

        public Task<string> SendNotification()
        {
            return _emailService.SendEmail(_emailViewModel);
        }
    }
}
