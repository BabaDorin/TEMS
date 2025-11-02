using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class UserCommonNotification
    {
        [ForeignKey("UserId")]
        public TEMSUser User { get; set; }
        public string UserId { get; set; }

        [ForeignKey("NotificationId")]
        public CommonNotification Notification { get; set; }

        [MaxLength(150)]
        public string NotificationId { get; set; }

        public bool Seen { get; set; }
    }
}
