using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers.EMail;
using temsAPI.Helpers.NotificationProviders;
using temsAPI.Services;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.Email;

namespace temsAPI.Controllers.EmailControllers
{
    public class EmailController : TEMSController
    {
        readonly EmailService _emailService;

        public EmailController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EmailService emailService,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _emailService = emailService;
        }

        [HttpPost("email/SendEmail")]
        [ClaimRequirement(TEMSClaims.CAN_SEND_EMAILS)]
        [DefaultExceptionHandler("An error occured while sending the email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailViewModel viewModel)
        {
            string validationResult = viewModel.Validate();
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Neutral);

            var recipients = (await _unitOfWork.Personnel
                .FindAll(
                    where: q => viewModel.Recipients.Contains(q.Id)
                    && !String.IsNullOrEmpty(q.Email),
                    select: q => EmailTo.FromPersonnel(q)))
                .ToList();

            var emailData = new EmailData()
            {
                From = viewModel.From,
                Recipients = recipients,
                Subject = viewModel.Subject,
                Text = viewModel.Text
            };

            var mailingResult = await new EmailNotificationProvider(_emailService, emailData).SendNotification();

            if (int.TryParse(mailingResult, out _))
                return ReturnResponse(mailingResult + " mails have been sent.", ResponseStatus.Success);
            else
                return ReturnResponse(mailingResult, ResponseStatus.Neutral);
        }
    }
}
