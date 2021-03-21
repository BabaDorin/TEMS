using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.UserEntities
{
    public class TemsJWT
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
