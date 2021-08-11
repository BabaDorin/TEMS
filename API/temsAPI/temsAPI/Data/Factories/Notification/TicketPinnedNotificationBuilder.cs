using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Factories.Notification;

namespace temsAPI.Data.Factories.Notification
{
    public class TicketPinnedNotificationBuilder : INotificationBuilder
    {
        Ticket _ticket;
        IEnumerable<TEMSUser> _recipients;

        public TicketPinnedNotificationBuilder(Ticket ticket, IEnumerable<TEMSUser> recipients)
        {
            _ticket = ticket;
            _recipients = recipients;
        }

        public CommonNotification Create() 
        {
            return new CommonNotification(
                    "A ticket has been pinned",
                    $"Someone pinned a ticket [Tr. No {_ticket.TrackingNumber}], make sure to check it out.",
                   _recipients,
                   sendEmail: false,
                   sendPush: false,
                   sendBrowser: false
                    );
        }
     
    }
}
