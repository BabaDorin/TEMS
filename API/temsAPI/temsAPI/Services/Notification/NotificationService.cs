using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Services.Notification
{
    public class NotificationService : INotificationService
    {
        public async Task<string> SendNotification(INotification notification)
        {
            // Create the collection of notification senders based on notification fields
            // Send notifications, collect results, return results.

            // Remove this on implementation
            await Task.Yield();

            return null;
        }
    }
}
