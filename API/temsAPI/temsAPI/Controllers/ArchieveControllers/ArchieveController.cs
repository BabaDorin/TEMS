using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.Repository;
using temsAPI.System_Files;
using temsAPI.ViewModels.Archieve;

namespace temsAPI.Controllers.ArchieveControllers
{
    public class ArchieveController : TEMSController
    {
        // BEFREE: Find a way to get rid of these switch statements via creating a smart way of accessing
        // the repo by item type.

        public ArchieveController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {

        }
        
        [HttpGet("/archieve/getarchieveditems/{itemType}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetArchievedItems(string itemType)
        {
            try
            {
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

        [HttpGet("archieve/setArchivationStatus/{itemType}/{itemId}/{status}")]
        public async Task<JsonResult> SetArchivationStatus(string itemType, string itemId, bool status)
        {
            try
            {
                string archivationStatus = null;
                ArchieveHelper archieveHelper = new ArchieveHelper(_userManager, _unitOfWork);
                switch (itemType.ToLower())
                {
                    case "equipment":
                        archivationStatus = await archieveHelper.SetEquipmentArchivationStatus(itemId, status);
                        break;
                    case "issues":
                        archivationStatus = await archieveHelper.SetTicketArchivationStatus(itemId, status);
                        break;
                    case "rooms":
                        archivationStatus = await archieveHelper.SetRoomArchivationStatus(itemId, status);
                        break;
                    case "personnel":
                        archivationStatus = await archieveHelper.SetPersonnelArchivationStatus(itemId, status);
                        break;
                    case "keys":
                        archivationStatus = await archieveHelper.SetKeyArchivationStatus(itemId, status);
                        break;
                    case "report templates":
                        archivationStatus = await archieveHelper.SetReportTemplateArchivationStatus(itemId, status);
                        break;
                    case "equipment allocations":
                        archivationStatus = await archieveHelper.SetEquipmenAllocationtArchivationStatus(itemId, status);
                        break;
                    case "logs":
                        archivationStatus = await archieveHelper.SetLogArchivationStatus(itemId, status);
                        break;
                    case "key allocations":
                        archivationStatus = await archieveHelper.SetKeyAllocationArchivationStatus(itemId, status);
                        break;
                    case "properties":
                        archivationStatus = await archieveHelper.SetPropertyArchivationStatus(itemId, status);
                        break;
                    case "equipment types":
                        archivationStatus = await archieveHelper.SetTypeArchivationStatus(itemId, status);
                        break;
                    case "equipment definitions":
                        archivationStatus = await archieveHelper.SetDefinitionArchivationStatus(itemId, status);
                        break;
                    default:
                        return ReturnResponse("Unknown items type", ResponseStatus.Fail);
                }

                if (archivationStatus != null)
                    return ReturnResponse(archivationStatus, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return ReturnResponse("An error occured while setting the archivation status", ResponseStatus.Fail);
            }
        }   
    }
}
