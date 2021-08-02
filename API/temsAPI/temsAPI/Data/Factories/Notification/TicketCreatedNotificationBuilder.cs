using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Factories.Notification
{
    public class TicketCreatedNotificationBuilder : INotificationBuilder
    {
        Ticket _ticket;
        List<string> _userIds;

        public TicketCreatedNotificationBuilder(Ticket ticket, List<string> userIds = null)
        {
            _ticket = ticket;
            _userIds = userIds;
        }

        public CommonNotification Create()
        {
            return new CommonNotification(
                "A ticket has been created",
                $"Someone needs help, make sure to check out the newly created ticket [Tr. No. {_ticket.TrackingNumber}]",
                _userIds,
                sendEmail: true,
                sendPush: true,
                sendBrowser: true
                );
        }
    }
}
