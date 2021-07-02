using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.UserEntities
{
    public class TemsJWT
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(1000)]
        public string Content { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
