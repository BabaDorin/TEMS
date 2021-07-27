using System;

namespace temsAPI.System_Files.Exceptions
{
    public enum ResponseStatus
    {
        Fail = 0,
        Success = 1,
        Neutral = 2
    }

    public class GenericException : Exception
    {
        public ResponseStatus Status { get; private set; }
        public object Additional { get; private set; }

        public GenericException(string message, ResponseStatus status, object additional = null)
            : base (message)
        {
            Status = status;
            Additional = additional;
        }
    }
}
