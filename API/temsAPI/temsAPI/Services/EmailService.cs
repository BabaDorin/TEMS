using FluentEmail.Core;
using FluentEmail.Smtp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.System_Files;
using temsAPI.ViewModels.Email;

namespace temsAPI.Services
{
    public class EmailService
    {
        private class EmailTo
        {
            public string Email { get; set; }
            public string Name { get; set; }
        }

        private IUnitOfWork _unitOfWork;
        private readonly AppSettings _appSettings;
        public EmailService(IUnitOfWork unitOfWork, AppSettings appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Sends an email according to the data provided via an instance of SendEmailViewModel.
        /// Returns a number representing then number of sent emails, or returns the error message in case of
        /// an error.
        /// </summary>
        /// <param name="emailData"></param>
        /// <returns></returns>
        public async Task<string> SendEmail(SendEmailViewModel emailData)
        {
            List<EmailTo> addresses = await GetEmailAddresses(emailData.Addressees);
            if (addresses == null || addresses.Count == 0)
                return "No email addresses found. No mail has been sent";

            SetDefaultSender();
            return await SendEmailsTo(addresses, emailData);
        }

        private async Task<string> SendEmailsTo(List<EmailTo> addresses, SendEmailViewModel emailData)
        {
            int numberOfEmailsSent = 0;
            foreach (var item in addresses)
            {
                var email = FluentEmail.Core.Email
                    .From(_appSettings.Email.EmailSenderAddress, emailData.From)
                    .To(item.Email, item.Name)
                    .Subject(emailData.Subject)
                    .Body(emailData.Text + "\n\n\nThis email has been send by TEMS platform. Cheers <3.");

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

        private async Task<List<EmailTo>> GetEmailAddresses(List<string> personnelIds)
        {
            try
            {
                List<EmailTo> addresses = (await _unitOfWork.Personnel
                    .FindAll<EmailTo>(
                        where: q => personnelIds.Contains(q.Id) && q.Email != null,
                        select: q => new EmailTo
                        {
                            Email = q.Email,
                            Name = q.Name
                        })).ToList();

                return addresses;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
