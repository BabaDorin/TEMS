using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Controllers;

namespace temsAPI.System_Files
{
    public class Response
    {
        public string Message { get; set; }
        public ResponseStatus Status { get; set; }
        public object Additional { get; set; }

        public Response(string message, ResponseStatus status, object additional = null)
        {
            Message = message;
            Status = status;
            Additional = additional;
        }
    }
}
