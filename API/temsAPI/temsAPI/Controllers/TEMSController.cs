using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;

namespace temsAPI.Controllers
{
    public enum ResponseStatus
    {
        Fail = 0,
        Success = 1,
        Neutral = 2
    }

    public abstract class TEMSController : Controller
    {
        protected static int maxConcurrentUploads = 2;
        protected static int concurrentUploads = 0;

        protected readonly IUnitOfWork _unitOfWork;
        protected readonly UserManager<TEMSUser> _userManager;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public TEMSController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            ILogger<TEMSController> logger
            )
        {
            _mapper = mapper;
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
