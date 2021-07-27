using System;

namespace temsAPI.System_Files.Exceptions.MethodData
{
    [Serializable]
    public class MethodArgument
    {
        public string ArgumentName { get; set; }
        public object ArgumentValue { get; set; }

        public MethodArgument(string argumentName, object argumentValue)
        {
            ArgumentName = argumentName;
            ArgumentValue = argumentValue;
        }
    }
}
