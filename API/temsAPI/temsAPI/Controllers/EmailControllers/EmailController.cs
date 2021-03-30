using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public EmailController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            IOptions<AppSettings> appSettings) : base(mapper, unitOfWork, userManager)
        {
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_SEND_EMAILS)]
        public async Task<JsonResult> SendEmail([FromBody] SendEmailViewModel viewModel)
        {
            try
            {
                var mailingResult = await (new EmailService(_unitOfWork, _appSettings)).SendEmail(viewModel);

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
