using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Services.Notification
{
    interface INotificationService
    {
        public Task<string> SendNotification(INotification notification);
    }
}
