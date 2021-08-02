using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Factories.Notification;

namespace temsAPI.Data.Factories.Notification
{
    public class TicketPinnedNotificationBuilder : INotificationBuilder
    {
        Ticket _ticket;
        List<string> _userIds;

        public TicketPinnedNotificationBuilder(Ticket ticket, List<string> userIds)
        {
            _ticket = ticket;
            _userIds = userIds;
        }

        public CommonNotification Create() 
        {
            return new CommonNotification(
                    "A ticket has been pinned",
                    $"Someone pinned a ticket [Tr. No {_ticket.TrackingNumber}], make sure to check it out.",
                   _userIds,
                   sendEmail: false,
                   sendPush: false,
                   sendBrowser: false
                    );
        }
     
    }
}
