using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.ViewModels.Notification
{
    public class NotificationViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }

        public static NotificationViewModel FromModel(INotification model)
        {
            return new NotificationViewModel()
            {
                Id = model.Id,
                DateCreated = model.DateCreated,
                Title = model.Title,
                Message = model.Message
            };
        }
    }
}
