﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace temsAPI.Data.Entities.UserEntities
{
    public class UserWithBlacklistedToken
    {
        [Key]
        [ForeignKey("User")]
        public string UserID { get; set; }
        public TEMSUser User { get; set; }

        /// <summary>
        /// Utc Now ONLY
        /// </summary>
        public DateTime DateBlacklisted { get; set; } = DateTime.UtcNow;
    }
}
