using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Options;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Helpers.ReusableSnippets;
using temsAPI.System_Files;
using temsAPI.ViewModels.Email;

namespace temsAPI.Services
{
    public class EmailService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly AppSettings _appSettings;

        public EmailService(
            IUnitOfWork unitOfWork, 
            IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        // BEFREE: Create email only once and send to multiple recipients
        public async Task<string> SendEmail(EmailData emailData)
        {
            SetDefaultSender();

            int numberOfEmailsSent = 0;
            foreach (var item in emailData.Recipients)
            {
                // Little addressee validation here, just to be sure
                if (item.Email.Length <= 3 || item.Email.IndexOf('@') <= 0)
                    continue;

                var email = FluentEmail.Core.Email
                    .From(_appSettings.Email.EmailSenderAddress, emailData.From)
                    .To(item.Email, item.Name)
                    .Subject(emailData.Subject)
                    .Body(emailData.Text + "\n\n\nThis email has been send by TEMS platform. Cheers <3.");

                if (!emailData.Attachments.IsNullOrEmpty())
                {
                    emailData.Attachments.ForEach(file =>
                    {
                        email.AttachFromFilename(file, null, Path.GetFileName(file));
                    });
                }

                await email.SendAsync();
                ++numberOfEmailsSent;
            }

            return numberOfEmailsSent.ToString();
        }

        private void SetDefaultSender()
        {
            var sender = new SmtpSender(() => new SmtpClient("smtp.gmail.com")
            {
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential(
                    _appSettings.Email.EmailSenderAddress,
                    _appSettings.Email.EmailSenderAddressPassword),
                EnableSsl = true

            });

            FluentEmail.Core.Email.DefaultSender = sender;
        }
    }
}
