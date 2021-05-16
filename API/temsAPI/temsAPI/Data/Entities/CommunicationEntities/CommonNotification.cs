using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class CommonNotification : BaseNotification
    {
        [Key]
        public string Id { get; set; }

        public List<TEMSUser> SendTo { get; set; }
    }
}
