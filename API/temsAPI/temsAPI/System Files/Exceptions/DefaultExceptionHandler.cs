using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Linq;
using System.Text;
using temsAPI.System_Files.Exceptions.MethodData;

namespace temsAPI.System_Files.Exceptions
{
    [PSerializable]
    public class DefaultExceptionHandler : OnExceptionAspect
    {
        public static ILogger logger;
        string _customErrorMessage;
        bool _ignoreArgumentLogging;

        public DefaultExceptionHandler(string customErrorMessage, bool ignoreArgumentLogging = false)
        {
            _customErrorMessage = customErrorMessage;
            _ignoreArgumentLogging = ignoreArgumentLogging;
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Log(args);

            var response = new Response(_customErrorMessage, ResponseStatus.Fail);

            args.ReturnValue = new ObjectResult(response)
            {
                StatusCode = 500
            };
            args.FlowBehavior = FlowBehavior.Return;
        }

        protected void Log(MethodExecutionArgs args)
        {
            // Exception report structure:
            // - [Date & Time] Exception message
            // - API endpoint where the exception has been thrown
            // - Method's arugments (JSON)
            // - Inner exception
            // - Stack trace

            var exception = args.Exception;
            var parameters = args.Method.GetParameters();

            StringBuilder additional = new StringBuilder();
            additional.Append("\n\n\n\n");

            // [Date | Time] Custom message (Exception message)
            additional.Append(DateTime.Now.ToString("[ dd.MM.yyyy | HH:mm:ss ]") + "   ");
            additional.Append(String.Format("{0} ({1})", _customErrorMessage, exception.Message));
            additional.Append("\n\n");

            // MethodName(Parameter names)
            additional.Append(args.Method.Name + "(");
            if (parameters != null)
                additional.Append(String.Join(",", parameters.Select(q => q.Name)));
            additional.Append(")");
            additional.Append("\n\n");

            // Parameters as JSON formatted list 
            if(parameters != null && !_ignoreArgumentLogging)
            {
                var parametersModel = new MethodExceptionData()
                {
                    Arguments = args.Arguments.Select(q => new MethodArgument("", q))
                };

                additional.Append("Actual parameters: \n");
                additional.Append(parametersModel.Serialize());
                additional.Append("\n\n");
            }

            // Inner exception (if any)
            if(exception.InnerException != null)
            {
                additional.Append("Inner exception:");
                additional.Append(exception.InnerException);

                additional.Append("\n\n- - - - - - - - - - - - - - -");
            }
            
            // Stack trace
            additional.Append("Stack trace:");
            additional.Append(exception.StackTrace);
         
            additional.Append("\n\n- - - - - - - - - - - - - - -");

            logger.Log(LogLevel.Error, additional.ToString());
        }
    }
}
