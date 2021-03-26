using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace temsAPI.Helpers
{
    public class IdentityHelper
    {
        public static string GetUserId(ClaimsPrincipal user)
        {
            return user.Claims.Where(q => q.Type == "UserID").Select(q => q.Value).SingleOrDefault();
        }
    }
}
