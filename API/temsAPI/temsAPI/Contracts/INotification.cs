using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Contracts
{
    public interface INotification
    {
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSMS { get; set; }
        public bool SendPush { get; set; }
        public bool SendBrowser { get; set; }

        public List<TEMSUser> GetUsers();
    }
}
