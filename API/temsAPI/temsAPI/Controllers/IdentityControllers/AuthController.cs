using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Controllers.IdentityControllers
{
    public class AuthController : TEMSController
    {
        public AuthController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [Authorize]
        [HttpGet]
        public JsonResult IsAuthenticated()
        {
            return ReturnResponse("yep", ResponseStatus.Success);
        }
    }
}
