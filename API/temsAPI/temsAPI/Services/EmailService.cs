using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
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
        private class EmailTo
        {
            public string Email { get; set; }
            public string Name { get; set; }

            public EmailTo()
            {

            }

            public EmailTo(string email, string name)
            {
                Email = email;
                Name = name;
            }
        }

        readonly IUnitOfWork _unitOfWork;
        readonly AppSettings _appSettings;
        public EmailService(
            IUnitOfWork unitOfWork, 
            IOptions<AppSettings> appSettings)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
        }

        // BEFREE: Something is wrong here, I feel it => DRY principle Violated

        /// <summary>
        /// Sends an email according to the data provided via an instance of SendEmailViewModel.
        /// Returns eather a number representing the number of sent emails, or returns an error message in case of
        /// an error.
        /// </summary>
        /// <param name="emailData"></param>
        /// <returns></returns>
        public async Task<string> SendEmailToPersonnel(SendEmailViewModel emailData)
        {
            List<EmailTo> addresses = await GetEmailAddresses_Personnel(emailData.Addressees);
            if (addresses.IsNullOrEmpty())
                return "No email addresses found. No mail has been sent";

            return await SendEmail(addresses, emailData);
        }

        public async Task<string> SendEmailToUsers(SendEmailViewModel emailData)
        {
            List<EmailTo> addresses = await GetEmailAddresses_Users(emailData.Addressees);
            if (addresses.IsNullOrEmpty())
                return "No email addresses found. No mail has been sent";
            
            return await SendEmail(addresses, emailData);
        }

        public async Task<string> SendEmailToAddresses(SendEmailViewModel emailData)
        {
            List<EmailTo> addressees = emailData.Addressees.Select(q => new EmailTo(q, q.Split('@')[0])).ToList();
            if (addressees.IsNullOrEmpty())
                return "No email addresses found. No mail has been sent";

            return await SendEmail(addressees, emailData);
        }


        private async Task<string> SendEmail(List<EmailTo> addresses, SendEmailViewModel emailData)
        {
            SetDefaultSender();
            return await SendEmailsTo(addresses, emailData);
        }

        // BEFREE: Create email only once and send to multiple recipients
        private async Task<string> SendEmailsTo(List<EmailTo> addresses, SendEmailViewModel emailData)
        {
            int numberOfEmailsSent = 0;
            foreach (var item in addresses)
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

        private async Task<List<EmailTo>> GetEmailAddresses_Personnel(List<string> addreseeIds)
        {
            return (await _unitOfWork.Personnel
                .FindAll<EmailTo>(
                    where: q => addreseeIds.Contains(q.Id) && !String.IsNullOrEmpty(q.Email),
                    select: q => new EmailTo
                    {
                        Email = q.Email,
                        Name = q.Name
                    })).ToList();
        }

        private async Task<List<EmailTo>> GetEmailAddresses_Users(List<string> addreseeIds)
        {
            List<EmailTo> addresses = (await _unitOfWork.TEMSUsers
                .FindAll<EmailTo>(
                    where: q => addreseeIds.Contains(q.Id) && !String.IsNullOrEmpty(q.Email),
                    select: q => new EmailTo
                    {
                        Email = q.Email,
                        Name = q.FullName ?? q.UserName
                    })).ToList();

            return addresses;
        }
    }
}
