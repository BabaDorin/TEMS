using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Helpers
{
    public class ReportHelper
    {
        public static List<string> UniversalProperties = new List<string>
        {
            "temsid", "serialNumber", "price", "currency", "purchaseDate", "description", "identifier"
        };
    }
}
