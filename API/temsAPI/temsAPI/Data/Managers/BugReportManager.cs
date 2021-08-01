using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Services;
using temsAPI.ViewModels.BugReport;

namespace temsAPI.Data.Managers
{
    public class BugReportManager
    {
        EmailService _emailService;
        
        public BugReportManager(EmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Processes the bug report, which includes saving the report & notifying 
        /// </summary>
        /// <returns>Null if everything is ok, otherwise - returns an error message</returns>
        public async Task<string> ProcessBugReport(BugReportViewModel viewModel)
        {
            throw new NotImplementedException();
        }
    }
}
