using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using temsAPI.Controllers;

namespace temsAPI.System_Files.TEMSFileLogger
{
    [PSerializable]
    public class PostSharpExceptionHandler: OnExceptionAspect
    {
        public static ILogger logger;
        string _customErrorMessage;

        public PostSharpExceptionHandler(string customErrorMessage)
        {
            _customErrorMessage = customErrorMessage;
        }

        public override void OnException(MethodExecutionArgs args)
        {
            if (logger == null)
                throw new Exception("Please, provide a logger instance.");

            args.ReturnValue = new BadHttpRequestException(_customErrorMessage, 500);

            //var response = new Response(_customErrorMessage, ResponseStatus.Fail);
            //throw new HttpRequestException()
            //base.OnException(args);
        }

        protected ILogger GetLoggerService(ExceptionContext context)
        {
            return (ILogger)context.HttpContext.RequestServices.GetService(typeof(ILogger));
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