using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.System_Files
{
    public class Email
    {
        public string EmailSenderAddress { get; set; }
        public string EmailSenderAddressPassword { get; set; }
    }

    public class AppSettings
    {
        public string JWT_Secret { get; set; }
        public string Client_Url { get; set; }
        public Email Email { get; set; }
    }
}
