using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;

namespace temsAPI.Controllers.ArchieveControllers
{
    public class ArchieveController : TEMSController
    {
        public ArchieveController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/archieve/getarchieveditemds/{itemType}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetArchievedItems(string itemType)
        {
            try
            {
                IGenericRepository<IArchiveable> archiveableCollection;

                //'Equipment',
                //'Issues',
                //'Rooms',
                //'Personnel',
                //'Keys',
                //'Report templates',
                //'Equipment Allocations',
                //'Equipment Logs',
                //'Room Logs',
                //'Personnel Logs',
                //'Key allocations',
                //'Properties',
                //'Types',
                //'Definitions',
                //switch (itemType.ToLower())
                //{
                //    case "equipment": return Json
                //}

                return Json("Nope");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while retrieveing archieved items", ResponseStatus.Fail);
            }
        }


    }
}
