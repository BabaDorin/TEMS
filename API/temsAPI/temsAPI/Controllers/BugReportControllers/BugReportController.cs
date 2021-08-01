using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.BugReport;

namespace temsAPI.Controllers.BugReportControllers
{
    public class BugReportController : Controller
    {
        [HttpPost("BugReport/SendReport")]
        [Authorize]
        [RequestSizeLimit(22_020_096)] // 21 MB
        [DefaultExceptionHandler("An error occured while sending the report")]
        public IActionResult SendReport ([FromForm] BugReportViewModel theFile)
        {
            var req = Request.Form;

            return Ok("nice");
        }
    }
}
