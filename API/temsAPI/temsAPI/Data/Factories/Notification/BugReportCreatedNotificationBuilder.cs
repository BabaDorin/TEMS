using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Factories.Notification
{
    public class BugReportCreatedNotificationBuilder : INotificationBuilder
    {
        BugReport _bugReport;
        List<string> _recipientIds;

        public BugReportCreatedNotificationBuilder(BugReport bugReport, List<string> recipientIds)
        {
            _bugReport = bugReport;
            _recipientIds = recipientIds;
        }

        public CommonNotification Create()
        {
            return new CommonNotification(
                "New bug report has been created",
                String.Format($"{_bugReport.CreatedBy.Identifier} created a new bug report. Make sure to check it out."),
                _recipientIds,
                sendPush: true
                );
        }
    }
}
