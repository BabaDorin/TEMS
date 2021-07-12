﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            LogManager logManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _logManager = logManager;
        }

        [HttpPost("log/Create")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Create([FromBody] AddLogViewModel viewModel)
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
                LogException(ex);
                return ReturnResponse("An error occured while creating the log record. Please try again", ResponseStatus.Fail);
            }
        }

        [HttpGet("/log/Archieve/{logId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Archieve(string logId, bool archivationStatus = true)
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
                LogException(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }

        [HttpGet("log/Remove/{logId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<IActionResult> Remove(string logId)
        {
            try
            {
                string result = await _logManager.Remove(logId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the log", ResponseStatus.Fail);
            }
        }

        [HttpGet("/log/GetEntityLogs/{entityType}/{entityId}/{pageNumber?}/{itemsPerPage?}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetEntityLogs(string entityType, string entityId, int? pageNumber, int? itemsPerPage)
        {
            try
            {
                int skip = (pageNumber != null && itemsPerPage != null)
                    ? (int)(itemsPerPage * (pageNumber - 1))
                    : 0;
                int take = (pageNumber != null && itemsPerPage != null)
                    ? (int)itemsPerPage
                    : int.MaxValue;

                var logs = await _logManager.GetEntityLogs(entityType, entityId, skip, take);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching entity logs", ResponseStatus.Fail);
            }
        }

        [HttpGet("log/GetItemsNumber/{entityType}/{entityId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetItemsNumber(string entityType, string entityId)
        {
            try
            {
                var number = await _logManager.GetItemsNumber(entityType, entityId);
                return Ok(number);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching log items number", ResponseStatus.Fail);
            }
        }
    }
}
