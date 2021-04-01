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
using temsAPI.Repository;
using temsAPI.System_Files;
using temsAPI.ViewModels;

namespace temsAPI.Controllers.ArchieveControllers
{
    public class ArchieveController : TEMSController
    {

        class ArchievedItem : IArchiveable, IIdentifiable
        {
            public bool IsArchieved { get; set; }
            public DateTime DateArchieved { get; set; }

            public string Identifier => "";

            public string Id { get; set; }
        }

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
                switch (itemType.ToLower())
                {
                    case "equipment": return Json(await getArchieved<Equipment>(_unitOfWork.Equipments));
                    case "issues": return Json(await getArchieved<Ticket>(_unitOfWork.Tickets));
                    case "rooms": return Json(await getArchieved<Room>(_unitOfWork.Rooms));
                    case "personnel": return Json(await getArchieved<Personnel>(_unitOfWork.Personnel));
                    case "keys": return Json(await getArchieved<Key>(_unitOfWork.Keys));
                    case "report templates": return Json(await getArchieved<ReportTemplate>(_unitOfWork.ReportTemplates));
                    case "equipment allocations": return Json(await getArchieved<EquipmentAllocation>(_unitOfWork.EquipmentAllocations));
                    case "logs": return Json(await getArchieved<Log>(_unitOfWork.Logs));
                    case "key allocations": return Json(await getArchieved<KeyAllocation>(_unitOfWork.KeyAllocations));
                    case "properties": return Json(await getArchieved<Property>(_unitOfWork.Properties));
                    case "equipment types": return Json(await getArchieved<EquipmentType>(_unitOfWork.EquipmentTypes));
                    case "equipment definitions": return Json(await getArchieved<EquipmentDefinition>(_unitOfWork.EquipmentDefinitions));
                    default:
                        return ReturnResponse("An error occured", ResponseStatus.Fail);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while retrieveing archieved items", ResponseStatus.Fail);
            }
        }

        public async Task<List<Option>> getArchieved<T>(IGenericRepository<T> repo) where T: class, IArchiveableItem
        {
            return (await repo.FindAll<Option>(
                    where: q => q.IsArchieved,
                    select: q => new Option
                    {
                        Label = q.Identifier,
                        Value = q.Id,
                        Additional = q.DateArchieved.ToString()
                    }
                )).ToList();
        }
    }
}
