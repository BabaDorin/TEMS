using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.ViewModels.Notification
{
    public class NotificationViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public bool Seen { get; set; }

        public static NotificationViewModel FromModel(INotification model, TEMSUser user)
        {
            return new NotificationViewModel()
            {
                Id = model.Id,
                DateCreated = model.DateCreated,
                Title = model.Title,
                Message = model.Message,
                Seen = model.IsSeen(user)
            };
        }
    }
}
