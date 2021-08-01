using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Factories.NotificationFactories
{
    public class TicketPinnedNotiFactory
    {
        public CommonNotification Create(Ticket ticket, List<string> userIds) 
        {
            return new CommonNotification(
                    "A ticket has been pinned",
                    $"Someone pinned a ticket [Tr. No {ticket.TrackingNumber}], make sure to check it out.",
                   userIds,
                   sendEmail: false,
                   sendPush: false,
                   sendBrowser: false
                    );
        }
     
    }
}
