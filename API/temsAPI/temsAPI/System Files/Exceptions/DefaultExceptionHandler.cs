using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Diagnostics;
using System.Text;

namespace temsAPI.System_Files.Exceptions
{
    [PSerializable]
    public class DefaultExceptionHandler : OnExceptionAspect
    {
        public static ILogger logger;
        string _customErrorMessage;

        public DefaultExceptionHandler(string customErrorMessage)
        {
            _customErrorMessage = customErrorMessage;
        }

        public override void OnException(MethodExecutionArgs args)
        {
            LogException(args.Exception);

            var response = new Response(_customErrorMessage, ResponseStatus.Fail);

            args.ReturnValue = new ObjectResult(response)
            {
                StatusCode = 500
            };
            args.FlowBehavior = FlowBehavior.Return;
        }

        protected void LogException(Exception ex, object caller = null, string header = null)
        {
            if (logger == null)
                throw new Exception("Please, provide a logger instance.");

            StringBuilder additional = new StringBuilder();

            if (caller != null)
                additional.Append(caller.GetType().Name + " - " + new StackTrace().GetFrame(1).GetMethod().Name);

            if (header != null)
                additional.Append(" " + header);

            logger.Log(LogLevel.Error, ex, additional.ToString() ?? "");
        }
    }
}
