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
using temsAPI.Data.Entities.EquipmentEntities;
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
                GenericRepository<ArchievedItem> archiveableCollection;

                IArchiveableItem item = new Equipment();
                         

                switch (itemType.ToLower())
                {
                    case "equipment": return Json(await getArchieved<Equipment>(_unitOfWork.Equipments));
                    default:
                        return ReturnResponse("An error occured", ResponseStatus.Fail);
                }
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

                //List<Option> viewModel = (await archiveableCollection.
                //    FindAll<Option>(
                //        where: q => q.IsArchieved,
                //        select: q => new Option
                //        {
                //            Value = q.Id,
                //            Label = q.Identifier,
                //            Additional = q.DateArchieved.ToString()
                //        })).ToList();

                return Json("1");
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
