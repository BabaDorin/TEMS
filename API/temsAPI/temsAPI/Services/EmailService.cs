using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.ViewModels.Email;

namespace temsAPI.Services
{
    public class EmailService
    {
        private IUnitOfWork _unitOfWork;
        public EmailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Sends an email according to the data provided via an instance of SendEmailViewModel.
        /// Returns null if everything is ok, otherwise - returns the error message.
        /// </summary>
        /// <param name="emailData"></param>
        /// <returns></returns>
        public async Task<string> SendEmail(SendEmailViewModel emailData)
        {
            return null;
        }
    }
}
