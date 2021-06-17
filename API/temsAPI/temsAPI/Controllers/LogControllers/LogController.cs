﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels.Log;

namespace temsAPI.Controllers.LogControllers
{
    public class LogController : TEMSController
    {
        private LogManager _logManager;

        public LogController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            LogManager logManager) : base(mapper, unitOfWork, userManager)
        {
            _logManager = logManager;
        }

        [HttpGet("/log/archieve/{logId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string logId, bool archivationStatus = true)
        {
            try
            {
                string archievingResult = await (new ArchieveHelper(_unitOfWork, User))
                    .SetLogArchivationStatus(logId, archivationStatus);
                if (archievingResult != null)
                    return ReturnResponse(archievingResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Create([FromBody] AddLogViewModel viewModel)
        {
            try
            {
                var result = await _logManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the log record. Please try again", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES, TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetLogTypes()
        {
            try
            {
                var logTypes = await _logManager.GetLogTypes();
                return Json(logTypes);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching log types", ResponseStatus.Fail);
            }
        }

        [HttpGet("/log/getentitylogs/{entityType}/{entityId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetEntityLogs(string entityType, string entityId)
        {
            try
            {
                var logs = await _logManager.GetEntityLogs(entityType, entityId);
                return Json(logs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching entity logs", ResponseStatus.Fail);
            }
        }
    }
}
