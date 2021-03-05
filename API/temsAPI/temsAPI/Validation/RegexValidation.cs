using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace temsAPI.Validation
{
    public class RegexValidation
    {
        public static Regex OnlyAlphaNumeric = new Regex("^[a-zA-Z0-9 ]*$");
    }
}
