using Microsoft.EntityFrameworkCore;
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
    public class UserNotification : BaseNotification
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("UserID")]
        public TEMSUser User { get; set; }
        public string UserID { get; set; }
    }
}
