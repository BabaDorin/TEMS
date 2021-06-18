using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.System_Files
{
    enum Currencies
    {
        Lei,
        Eur,
        USD
    };

    public static class HardCodedValues
    {
        public static List<string> EntityTypes 
            => new List<string>() { "room", "personnel", "equipment", "user" };

    }
}
