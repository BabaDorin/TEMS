using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;

namespace temsAPI.Controllers
{
    public abstract class TEMSController : Controller
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly UserManager<TEMSUser> _userManager;
        protected readonly ILogger _logger;

        public TEMSController(
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            ILogger<TEMSController> logger
            )
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        protected void LogException(Exception ex, object caller = null, string header = null)
        {
            StringBuilder additional = new StringBuilder();
            
            if(caller != null)
                additional.Append(caller.GetType().Name + " - " + new StackTrace().GetFrame(1).GetMethod().Name);

            if (header != null)
                additional.Append(" " + header);

            _logger.Log(LogLevel.Error, ex, additional.ToString() ?? "");
        }

        protected IActionResult ReturnResponse(string message, ResponseStatus status, object additional = null)
        {
            var response = new Response(message, status, additional);

            if (status == ResponseStatus.Success || status == ResponseStatus.Neutral)
                return Ok(response);
            
            return StatusCode(500, response);
        }
    }
}
