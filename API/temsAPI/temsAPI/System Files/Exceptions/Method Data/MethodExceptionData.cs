using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace temsAPI.System_Files.Exceptions.MethodData
{
    [Serializable]
    public class MethodExceptionData
    {
        public IEnumerable<MethodArgument> Arguments { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
