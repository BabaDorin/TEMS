using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Factories.Notification
{
    public class BugReportCreatedNotificationBuilder : INotificationBuilder
    {
        BugReport _bugReport;
        IEnumerable<TEMSUser> _recipients;

        public BugReportCreatedNotificationBuilder(BugReport bugReport, IEnumerable<TEMSUser> recipients)
        {
            _bugReport = bugReport;
            _recipients = recipients;
        }

        public CommonNotification Create()
        {
            return new CommonNotification(
                "New bug report has been created",
                String.Format($"{_bugReport.CreatedBy.Identifier} created a new bug report. Make sure to check it out."),
                _recipients,
                sendPush: true
                );
        }
    }
}
