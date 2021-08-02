using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Factories.Notification;

namespace temsAPI.Data.Factories.Notification
{
    public class TicketAssignedNotificationBuilder : INotificationBuilder
    {
        Ticket _ticket;
        List<string> _userIds;

        public TicketAssignedNotificationBuilder(Ticket ticket, List<string> userIds = null)
        {
            _ticket = ticket;
            _userIds = userIds;
        }

        public CommonNotification Create()
        {
            if (_userIds == null)
                _userIds = _ticket.Assignees.Select(q => q.Id).ToList();

            return new CommonNotification(
                "You've been assigned a ticket",
                $"A ticket [Tr. No. {_ticket.TrackingNumber}] has been assigned to you, make sure to check it out.",
                _userIds,
                sendEmail: true,
                sendPush: true,
                sendBrowser: true
                );
        }
    }
}
