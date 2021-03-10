﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
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
                    return ReturnResponse("We could not find any equipment having the specifid Id", ResponseStatus.Fail);

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
                return ReturnResponse("An error occured when fetching equipment logs", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddLogViewModel viewModel)
        {
            try
            {
                // Invalid AddresseesType
                List<string> validAddresseesTypes = new List<string> { "equipment", "room", "personnel" };
                if(validAddresseesTypes.IndexOf(viewModel.AddresseesType) == -1)
                    return ReturnResponse("Please, provide a valid Addressee Type", ResponseStatus.Fail);

                // No Addressees provided or the provided ones are invalid
                if (viewModel.Addressees.Count == 0)
                    return ReturnResponse("Please, provide at least one Addressee", ResponseStatus.Fail);

                switch (viewModel.AddresseesType)
                {
                    case "equipment":
                        foreach (Option option in viewModel.Addressees)
                            if(!await _unitOfWork.Equipments.isExists(eq => eq.Id == option.Value))
                                return ReturnResponse("One or more addressee are invalid.", ResponseStatus.Fail);
                        break;

                    case "room":
                        foreach (Option option in viewModel.Addressees)
                            if (!await _unitOfWork.Rooms.isExists(eq => eq.Id == option.Value))
                                return ReturnResponse("One or more addressee are invalid.", ResponseStatus.Fail);
                        break;

                    case "personnel":
                        foreach (Option option in viewModel.Addressees)
                            if (!await _unitOfWork.Personnel.isExists(eq => eq.Id == option.Value))
                                return ReturnResponse("One or more addressee are invalid.", ResponseStatus.Fail);
                        break;
                }

                // No LogTypeId provided or the provided one is invalid
                if (String.IsNullOrEmpty(viewModel.LogTypeId))
                    return ReturnResponse("Please, Provide a log type for the log", ResponseStatus.Fail);

                if(!await _unitOfWork.LogTypes.isExists(q => q.Id == viewModel.LogTypeId))
                    return ReturnResponse("The provided Log type is invalid", ResponseStatus.Fail);

                // If we got so far, It might be valid
                List<string> addresseesWhereFailed = new List<string>();
                foreach (Option addressee in viewModel.Addressees)
                {
                    Log log = new Log
                    {
                        Id = Guid.NewGuid().ToString(),
                        DateCreated = DateTime.Now,
                        IsImportant = viewModel.IsImportant,
                        LogTypeID = viewModel.LogTypeId,
                        Text = viewModel.Text,
                    };

                    switch (viewModel.AddresseesType)
                    {
                        case "equipment": log.EquipmentID = addressee.Value; break;
                        case "room": log.RoomID = addressee.Value; break;
                        case "personnel": log.PersonnelID = addressee.Value; break;
                    }

                    await _unitOfWork.Logs.Create(log);
                    await _unitOfWork.Save();

                    if (!await _unitOfWork.Logs.isExists(q => q.Id == log.Id))
                        addresseesWhereFailed.Add(addressee.Label);
                }

                if (addresseesWhereFailed.Count > 0)
                    return ReturnResponse(
                        "Could not create log for the following identities:" + string.Join(",", addresseesWhereFailed),
                        ResponseStatus.Fail);
                else
                    return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception)
            {
                return ReturnResponse("An error occured when creating the log record. Please try again", ResponseStatus.Fail);
            }
        }

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
                return ReturnResponse("An error occured when fetching log types", ResponseStatus.Fail);
            }
        }
    }
}