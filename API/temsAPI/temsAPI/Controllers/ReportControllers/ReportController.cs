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
using temsAPI.System_Files;
using temsAPI.ViewModels.Report;

namespace temsAPI.Controllers.ReportControllers
{
    public class ReportController : TEMSController
    {
        public ReportController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        //[HttpPost]
        //[ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        //public async Task<JsonResult> AddTemplate([FromBody] AddReportTemplateViewModel viewModel)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //        return ReturnResponse("An error occured while saving the template", ResponseStatus.Fail);
        //    }
        //}


        // -----------------------------------------------------------------------

        /// <summary>
        /// Validates an instance of AddReportViewModel. Return null if everything is ok, otherwise - 
        /// returns an error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        //private async Task<string> ValidateTemplate(AddReportTemplateViewModel viewModel)
        //{
        //    // Invalid id provided (When it's the udpate case)
        //    if(viewModel.Id != null && !await _unitOfWork.repo)
        //}
    }
}
