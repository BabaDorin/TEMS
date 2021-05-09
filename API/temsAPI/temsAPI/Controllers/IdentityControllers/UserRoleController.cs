using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files;
using temsAPI.ViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class UserRoleController : TEMSController
    {
        TEMSUserManager _temsUserManager;
        public UserRoleController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            TEMSUserManager temsUserManager) : base(mapper, unitOfWork, userManager)
        {
            _temsUserManager = temsUserManager;
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> GetAllAutocompleteOptions()
        {
            try
            {
                var roles = await _temsUserManager.GetRolesOptions();
                return Json(roles);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching roles", ResponseStatus.Fail);
            }
        }
    }
}
