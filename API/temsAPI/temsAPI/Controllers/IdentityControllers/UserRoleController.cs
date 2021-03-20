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
using temsAPI.ViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class UserRoleController : TEMSController
    {
        private RoleManager<IdentityRole> _roleManager;
        public UserRoleController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager) : base(mapper, unitOfWork, userManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<JsonResult> GetAllAutocompleteOptions()
        {
            try
            {
                List<Option> roles = await _roleManager
                    .Roles
                    .Select(q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }).ToListAsync();

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
