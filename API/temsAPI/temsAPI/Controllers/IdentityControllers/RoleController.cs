using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.IdentityViewModels;

namespace temsAPI.Controllers.IdentityControllers
{
    public class RoleController : TEMSController
    {
        private RoleManager<IdentityRole> _roleManager;

        public RoleController(
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _roleManager = roleManager;
        }

        [HttpGet("role/GetRoles")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while fetching roles")]
        public IActionResult GetRoles()
        {
            List<ViewRoleViewModel> roles = _roleManager.Roles
               .Select(q => ViewRoleViewModel.FromModel(q, _roleManager))
               .ToList();

            return Ok(roles);
        }
    }
}
