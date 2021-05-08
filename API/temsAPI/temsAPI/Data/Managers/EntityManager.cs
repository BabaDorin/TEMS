using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Data.Managers
{
    public abstract class EntityManager
    {
        protected IUnitOfWork _unitOfWork;
        protected ClaimsPrincipal _user;

        public EntityManager(IUnitOfWork unitOfWork, ClaimsPrincipal user)
        {
            _unitOfWork = unitOfWork;
            _user = user;
        }
    }
}
