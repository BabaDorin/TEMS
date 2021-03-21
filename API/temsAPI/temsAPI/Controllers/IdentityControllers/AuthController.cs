using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;

namespace temsAPI.Controllers.IdentityControllers
{
    public class AuthController : TEMSController
    {
        public AuthController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet]
        public JsonResult IsAuthenticated()
        {
            return ReturnResponse("yep", ResponseStatus.Success);
        }

        [HttpPost]
        public async Task<IActionResult> SignOut([FromBody] string token)
        {
            try
            {
                // We make use of user here a little bit ;)
                List<TemsJWT> toRemove = (await _unitOfWork.JWTBlacklist
                    .FindAll<TemsJWT>(
                        where: q => q.ExpirationDate < DateTime.Now
                    )).ToList();

                foreach (TemsJWT jwt in toRemove)
                {
                    _unitOfWork.JWTBlacklist.Delete(jwt);
                }

                JWTHelper jwtHelper = new JWTHelper();

                await _unitOfWork.JWTBlacklist.Create(
                    new TemsJWT
                    {
                        Id = Guid.NewGuid().ToString(),
                        Content = token,
                        ExpirationDate = jwtHelper.GetExpiryTimestamp(token)
                    });

                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success); // call from frontent
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when invalidating token", ResponseStatus.Fail);
            }
        }

    }
}
