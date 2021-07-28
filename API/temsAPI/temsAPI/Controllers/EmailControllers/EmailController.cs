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
        private readonly AppSettings _appSettings;
        private EmailService _emailService;

        public EmailController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            IOptions<AppSettings> appSettings,
            EmailService emailService,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _appSettings = appSettings.Value;
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
