using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.CommunicationEntities;

namespace temsAPI.Data.Factories.Notification
{
    interface INotificationBuilder
    {
        public CommonNotification Create();
    }
}
