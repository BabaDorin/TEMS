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
        IEnumerable<TEMSUser> _recipients;

        public TicketAssignedNotificationBuilder(Ticket ticket, IEnumerable<TEMSUser> recipients)
        {
            _ticket = ticket;
            _recipients = recipients;
        }

        public CommonNotification Create()
        {
            if (_recipients == null)
                throw new Exception("No recipients specified for the notification");

            return new CommonNotification(
                "You've been assigned a ticket",
                $"A ticket [Tr. No. {_ticket.TrackingNumber}] has been assigned to you, make sure to check it out.",
                _recipients,
                sendEmail: true,
                sendPush: true,
                sendBrowser: true
                );
        }
    }
}
