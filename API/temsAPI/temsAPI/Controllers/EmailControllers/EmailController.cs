using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Services;
using temsAPI.System_Files;
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

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_SEND_EMAILS)]
        public async Task<JsonResult> SendEmail([FromBody] SendEmailViewModel viewModel)
        {
            try
            {
                var mailingResult = await _emailService.SendEmail(viewModel);
                
                int numbersOfEmailsSent = 0;
                if (int.TryParse(mailingResult, out numbersOfEmailsSent))
                    return ReturnResponse(mailingResult + " mails have been sent.", ResponseStatus.Success);
                else
                    return ReturnResponse(mailingResult, ResponseStatus.Fail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while sending the email", ResponseStatus.Fail);
            }
        }
    }
}
