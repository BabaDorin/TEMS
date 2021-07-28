using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
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
            var mailingResult = await _emailService.SendEmailToPersonnel(viewModel);

            if (int.TryParse(mailingResult, out _))
                return ReturnResponse(mailingResult + " mails have been sent.", ResponseStatus.Success);
            else
                return ReturnResponse(mailingResult, ResponseStatus.Neutral);
        }
    }
}
