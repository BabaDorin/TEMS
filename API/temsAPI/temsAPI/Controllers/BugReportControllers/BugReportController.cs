using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostSharp.Aspects.Advices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.BugReport;

namespace temsAPI.Controllers.BugReportControllers
{
    public class BugReportController : TEMSController
    {
        private BugReportManager _bugReportManager;

        public BugReportController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager, 
            ILogger<TEMSController> logger,
            BugReportManager bugReportManager) : base(unitOfWork, userManager, logger)
        {
            _bugReportManager = bugReportManager;
        }

        [HttpPost("BugReport/SendReport")]
        [Authorize]
        [RequestSizeLimit(22_020_096)] // 21 MB
        [DefaultExceptionHandler("An error occured while sending the report")]
        public async Task<IActionResult> SendReport (BugReportViewModel viewModel)
        {
            var result = await _bugReportManager.ProcessBugReport(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("BugReport/GetFullReport")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while fetching report data")]
        public async Task<IActionResult> GetFullReport(string reportId)
        {
            var report = await _bugReportManager.GetFullBugReport(reportId);
            if (report == null)
                return ReturnResponse("Invalid report id provided", ResponseStatus.Neutral);

            return Ok(report);
        }

        [HttpGet("BugReport/GetBugReportsSimplified")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while fetching reports")]
        public async Task<IActionResult> GetBugReportsSimplified(int skip, int take)
        {
            var reports = await _bugReportManager.GetBugReportsSimplified(skip, take);
            return Ok(reports);
        }

        [HttpGet("BugReport/GetTotalBugReportsAmount")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while fetching total amount of reports")]
        public async Task<IActionResult> GetTotalBugReportsAmount()
        {
            var amount = await _bugReportManager.GetTotalBugReportsAmount();
            return Ok(amount);
        }

        [HttpDelete("BugReport/RemoveReport")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while fetching total amount of reports")]
        public async Task<IActionResult> RemoveReport(string reportId)
        {
            string result = await _bugReportManager.RemoveReport(reportId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("BugReport/FetchAttachment")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while fetching report's attachment")]
        public async Task<IActionResult> FetchAttachment(string reportId, int attachmentIndex)
        {
            var attachmentData = await _bugReportManager.GetAttachmentData(reportId, attachmentIndex);
            return File(attachmentData.Item1, attachmentData.Item2);
        }
    }
}
