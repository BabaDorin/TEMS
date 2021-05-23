using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Factories.NotificationFactories
{
    public class TicketAssignedNotiFactory
    {
        public CommonNotification Create(Ticket ticket, List<string> userIds = null)
        {
            if (userIds == null)
                userIds = ticket.Assignees.Select(q => q.Id).ToList();

            return new CommonNotification(
                "You've been assigned a ticket",
                "A ticket has been created in which you figure as assignee",
                userIds,
                sendEmail: true,
                sendPush: true,
                sendBrowser: true
                );
        }
    }
}
