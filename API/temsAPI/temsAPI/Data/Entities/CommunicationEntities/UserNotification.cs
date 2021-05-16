using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    [Index(nameof(UserID))]
    public class UserNotification : INotification
    {
        [Key]
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSMS { get; set; }
        public bool SendPush { get; set; }
        public bool SendBrowser { get; set; }
        
        [ForeignKey("UserID")]
        public TEMSUser User { get; set; }
        public string UserID { get; set; }

        public List<TEMSUser> GetUsers()
        {
            List<TEMSUser> users = new();
            if (this.User != null)
                users.Add(User);

            return users;
        }
    }
}
