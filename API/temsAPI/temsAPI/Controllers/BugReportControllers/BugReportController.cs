using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
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
        public async Task<IActionResult> SendReport ([FromForm] BugReportViewModel viewModel)
        {
            var result = await _bugReportManager.ProcessBugReport(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }
    }
}
