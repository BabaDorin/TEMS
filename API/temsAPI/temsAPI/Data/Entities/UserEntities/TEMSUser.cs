using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.UserEntities
{
    public class TEMSUser : IdentityUser
    {
        public bool IsArchieved { get; set; }
    }
}
