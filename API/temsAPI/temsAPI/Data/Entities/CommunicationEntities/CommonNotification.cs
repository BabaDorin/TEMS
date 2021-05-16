using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class CommonNotification : INotification
    {
        [Key]
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }
        public string Message { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSMS { get; set; }
        public bool SendPush { get; set; }
        public bool SendBrowser { get; set; }
        public List<TEMSUser> SendTo { get; set; } = new List<TEMSUser>();

        public List<TEMSUser> GetUsers() => SendTo;
    }
}
