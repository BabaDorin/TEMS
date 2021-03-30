using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public EmailController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_SEND_EMAILS)]
        public async Task<JsonResult> SendEmail([FromBody] SendEmailViewModel viewModel)
        {
            try
            {
                var mailingResult = await (new EmailService(_unitOfWork)).SendEmail(viewModel);
                if (mailingResult != null)
                    return ReturnResponse(mailingResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while sending the email", ResponseStatus.Fail);
            }
        }
    }
}
