using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Data.Managers
{
    public class NotificationManager : EntityManager
    {
        public NotificationManager(IUnitOfWork unitOfWork, ClaimsPrincipal user) : base(unitOfWork, user)
        {
        }
    }
}
