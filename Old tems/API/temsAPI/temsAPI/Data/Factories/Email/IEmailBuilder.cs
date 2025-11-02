using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.ViewModels.Email;

namespace temsAPI.Data.Factories.Email
{
    interface IEmailBuilder
    {
        public Task<EmailData> BuildEmailModel();
    }
}
