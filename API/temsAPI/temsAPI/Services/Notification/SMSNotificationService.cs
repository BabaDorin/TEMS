using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Services.Notification
{
    public class SMSNotificationService : INotificationService
    {
        public async Task<string> SendNotification(INotification notification)
        {
            throw new NotImplementedException();
        }
    }
}
