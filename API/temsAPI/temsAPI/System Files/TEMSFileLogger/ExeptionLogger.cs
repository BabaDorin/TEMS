using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using temsAPI.Controllers;

namespace temsAPI.System_Files.TEMSFileLogger
{
    public class DefaultExceptionLogger : ExceptionFilterAttribute
    {
        readonly string _customErrorMessage;

        public DefaultExceptionLogger(string customErrorMessage = "An error occured")
        {
            _customErrorMessage = customErrorMessage;
        }

        public override void OnException(ExceptionContext context)
        {
            ILogger<TEMSController> logger = GetLoggerService(context);
            LogException(logger, context.Exception, null, _customErrorMessage);

            // Assign the 500 status code for any exception type for now.
            var response = new Response(_customErrorMessage, ResponseStatus.Fail);
            context.Result = new ObjectResult(response)
            {
                StatusCode = 500
            };
            
            base.OnException(context);
        }

        protected ILogger<TEMSController> GetLoggerService(ExceptionContext context)
        {
            return (ILogger<TEMSController>)context.HttpContext.RequestServices.GetService(typeof(ILogger<TEMSController>));
        }

        protected void LogException(ILogger logger, Exception ex, object caller = null, string header = null)
        {
            StringBuilder additional = new StringBuilder();

            if (caller != null)
                additional.Append(caller.GetType().Name + " - " + new StackTrace().GetFrame(1).GetMethod().Name);

            if (header != null)
                additional.Append(" " + header);

            logger.Log(LogLevel.Error, ex, additional.ToString() ?? "");
        }
    }
}