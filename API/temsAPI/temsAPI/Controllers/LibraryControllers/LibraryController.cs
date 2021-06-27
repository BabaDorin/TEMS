﻿using AutoMapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers.AnalyticsHelpers.AnalyticsModels;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services;
using temsAPI.System_Files;

namespace temsAPI.Controllers.LibraryControllers
{
    public class LibraryController : TEMSController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private LibraryItemFileHandler fileHandler = new();
        private LibraryManager _libraryManager;
        private IdentityService _identityService;
        private SystemConfigurationService _systemConfigurationService;

        public LibraryController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IdentityService identityService,
            SystemConfigurationService systemConfigurationService,
            LibraryManager libraryManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _libraryManager = libraryManager;
            _identityService = identityService;
            _systemConfigurationService = systemConfigurationService;
        }

        [HttpPost, DisableRequestSizeLimit]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                // 1: Check if there some space available

                var availableSpace = _libraryManager.GetAvailableSpace_bytes();
                if (availableSpace <= 0)
                    return StatusCode(500, "There is no more available space allocated for library storage. Free up some space first.");

                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                // 2: Check if the current file size won't overflow available library storage space
                if (availableSpace - file.Length <= 0)
                    return StatusCode(500, "Free up some space first.");

                var fileName = Request.Form["myName"];
                var fileDescription = Request.Form["myDescription"];
                var result = await _libraryManager.UploadFile(file, fileName, fileDescription);

                if (result != null)
                    return StatusCode(500, result);

                return Ok();
            }
            catch (ConnectionResetException ex)
            {
                // When the Client - Server connection has been "closed" (Where closed means that
                // the client stopped sending requests aka When TEMS Tab is closed or user canceled the upload,
                // this Exeption will be trown.

                // This behaviour helps us to easily implement the "cancel upload" feature (kind of), without
                // Third party Upload Controllers that cost money ;(
                LogException(ex);
                return Ok();
            }
            catch (Exception ex)
            {
                LogException(ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("library/getlibraryitems/{accessPassword?}")]
        public async Task<JsonResult> GetLibraryItems(string accessPassword)
        {
            try
            {
                if (!_identityService.IsAuthenticated() && accessPassword != _systemConfigurationService.AppSettings.LibraryGuestPassword)
                    return ReturnResponse("Incorrect password.", ResponseStatus.Fail);

                var items = await _libraryManager.GetItems();
                return Json(items);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching library items", ResponseStatus.Fail);
            }
        }

        [HttpGet("library/remove/{itemId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Remove(string itemId)
        {
            try
            {
                var result = await _libraryManager.Remove(itemId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when removing an library item.", ResponseStatus.Fail);
            }
        }

        [HttpGet("library/download/{itemId}"), DisableRequestSizeLimit]
        public async Task<IActionResult> Download(string itemId)
        {
            var item = await _libraryManager.GetById(itemId);
            var memoryStream = await _libraryManager.GetLibraryItemMemoryStream(item);

            string filePath = _libraryManager.GetFilePath(item);
            var file = File(memoryStream, fileHandler.GetContentType(filePath), item.ActualName + ".zip");

            // Update downloads counter
            await Task.Run(async () =>
            {
                await _libraryManager.UpdateDownloadsCounter(item);
            }).ConfigureAwait(false);

            return file;
        }

        [HttpGet]
        public JsonResult GetSpaceUsageData()
        {
            try
            {
                double usedSpaceBytes = StaticFileHelper.DirSizeBytes(new System.IO.DirectoryInfo(fileHandler.FolderPath));
                double usedSpaceGb = usedSpaceBytes / 1073741824;
                
                Fraction fraction = new();
                fraction.Numerator = usedSpaceGb;
                fraction.Denominator = _systemConfigurationService.AppSettings.LibraryAllocatedStorageSpaceGb;

                return Json(fraction);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching space usage data.", ResponseStatus.Fail);
            }
        }
       
        [HttpGet]
        public JsonResult GetAvailableLibraryStorageSpace()
        {
            try
            {
                return Json(_libraryManager.GetAvailableSpace_bytes());
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching available library storage space.", ResponseStatus.Fail);
            }
        }
        
    }
}
