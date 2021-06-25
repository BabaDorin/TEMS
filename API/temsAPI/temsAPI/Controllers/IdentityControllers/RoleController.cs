using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class RoleController : TEMSController
    {

        private RoleManager<IdentityRole> _roleManager;

        public RoleController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public JsonResult GetRoles()
        {
            try
            {
                List<ViewRoleViewModel> roles = _roleManager.Roles
               .Select(q => ViewRoleViewModel.FromModel(q, _roleManager))
               .ToList();

                return Json(roles);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching roles", ResponseStatus.Fail);
            }
        }
    }
}
