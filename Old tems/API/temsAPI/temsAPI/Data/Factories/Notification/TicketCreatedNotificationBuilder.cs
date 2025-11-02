using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Factories.Notification
{
    public class TicketCreatedNotificationBuilder : INotificationBuilder
    {
        Ticket _ticket;
        IEnumerable<TEMSUser> _recipients;

        public TicketCreatedNotificationBuilder(Ticket ticket, IEnumerable<TEMSUser> recipients)
        {
            _ticket = ticket;
            _recipients = recipients;
        }

        public CommonNotification Create()
        {
            return new CommonNotification(
                "A ticket has been created",
                $"Someone needs help, make sure to check out the newly created ticket [Tr. No. {_ticket.TrackingNumber}]",
                _recipients,
                sendEmail: true,
                sendPush: true,
                sendBrowser: true
                );
        }
    }
}
