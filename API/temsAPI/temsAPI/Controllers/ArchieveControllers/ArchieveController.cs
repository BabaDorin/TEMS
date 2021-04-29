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
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.KeyEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.Repository;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Archieve;

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

        [HttpGet("/archieve/getarchieveditems/{itemType}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetArchievedItems(string itemType)
        {
            try
            {
                var archievedItems = new List<ArchievedItemViewModel>();
                switch (itemType.ToLower())
                {
                    case "equipment":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Equipments));
                    case "issues":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Tickets));
                    case "rooms":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Rooms));
                    case "personnel":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Personnel));
                    case "keys":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Keys));
                    case "report templates":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.ReportTemplates));
                    case "equipment allocations":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.EquipmentAllocations));
                    case "logs":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Logs));
                    case "key allocations":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.KeyAllocations));
                    case "properties":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.Properties));
                    case "equipment types":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.EquipmentTypes));
                    case "equipment definitions":
                        return Json(await ArchieveHelper.GetArchievedItemsFromRepo(_unitOfWork.EquipmentDefinitions));
                    default:
                        return ReturnResponse("Unknown items type", ResponseStatus.Fail);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while retrieveing archieved items", ResponseStatus.Fail);
            }
        }
    }
}
