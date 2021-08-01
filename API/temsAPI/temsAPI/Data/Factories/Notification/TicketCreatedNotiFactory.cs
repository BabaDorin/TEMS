using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Factories.NotificationFactories
{
    public class TicketCreatedNotiFactory
    {
        public CommonNotification Create(Ticket ticket, List<string> userIds)
        {
            return new CommonNotification(
                "A ticket has been created",
                $"Someone needs help, make sure to check out the newly created ticket [Tr. No. {ticket.TrackingNumber}]",
                userIds,
                sendEmail: true,
                sendPush: true,
                sendBrowser: true
                );
        }
    }
}
