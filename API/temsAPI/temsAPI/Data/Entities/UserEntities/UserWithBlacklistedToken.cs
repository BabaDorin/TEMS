using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.UserEntities
{
    public class UserWithBlacklistedToken
    {
        [Key]
        public string UserID { get; set; }
        public DateTime DateBlacklisted { get; set; } = DateTime.Now;
    }
}
