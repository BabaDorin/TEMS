using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Log;

namespace temsAPI.Controllers.CommunicationControllers
{
    public class LogController : TEMSController
    {
        public LogController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet("/log/equipment/{equipmentId}/{includeArchieved}/{onlyArchieved}")]
        public async Task<JsonResult> GetEquipmentLogs(string equipmentId, bool? includeArchieved, bool? onlyArchieved)
        {
            try
            {
                // Invalid eqId
                if (!await _unitOfWork.Equipments.isExists(q => q.Id == equipmentId))
                    return ReturnResponse("We could not find any equipment having the specifid Id", Status.Fail);

                Expression<Func<Log, bool>> expression = null;

                expression = q => q.EquipmentID == equipmentId && !q.IsArchieved;

                if ((bool)includeArchieved)
                    expression = q => q.EquipmentID == equipmentId;

                if ((bool)onlyArchieved)
                    expression = q => q.EquipmentID == equipmentId && q.IsArchieved;

                List<ViewLogViewModel> viewModel = (await _unitOfWork.Logs.FindAll<ViewLogViewModel>(
                    where: expression,
                    include: q => q.Include(q => q.Equipment)
                    .ThenInclude(q => q.EquipmentDefinition)
                    .Include(q => q.Room)
                    .Include(q => q.Personnel)
                    .Include(q => q.LogType),
                    select: q => new ViewLogViewModel
                    {
                        DateCreated = q.DateCreated,
                        Equipment = new Option
                        {
                            Value = q.Equipment.Id,
                            Label = q.Equipment.TemsIdOrSerialNumber,
                            Additional = q.Equipment.EquipmentDefinition.Identifier
                        },
                        Id = q.Id,
                        IsImportant = q.IsImportant,
                        LogType = new Option
                        {
                            Value = q.LogType.Id,
                            Label = q.LogType.Type
                        },
                        Personnel = (q.Personnel != null)
                                    ? new Option { Value = q.Personnel.Id, Label = q.Personnel.Name }
                                    : new Option { },
                        Room = (q.Room != null)
                                    ? new Option { Value = q.Room.Id, Label = q.Room.Identifier }
                                    : new Option { },
                        Text = q.Text
                    })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching equipment logs", Status.Fail);
            }
        }

        //[HttpPost]
        //public async Task<JsonResult> CreateLog([FromBody] AddLogViewModel viewModel)
        //{
        //    try
        //    {

        //    }
        //    catch (Exception)
        //    {
        //        return ReturnResponse("An error occured when ")
        //    }
        //}

        [HttpGet]
        public async Task<JsonResult> GetLogTypes()
        {
            try
            {
                List<Option> logTypes = (await _unitOfWork.LogTypes
                    .FindAll<Option>(
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Type
                        }
                    )).ToList();

                return Json(logTypes);
            }
            catch (Exception)
            {
                return ReturnResponse("An error occured when fetching log types", Status.Fail);
            }
        }
    }
}
