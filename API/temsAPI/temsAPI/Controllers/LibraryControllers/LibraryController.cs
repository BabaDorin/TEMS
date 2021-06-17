using AutoMapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.System_Files;

namespace temsAPI.Controllers.LibraryControllers
{
    public class LibraryController : TEMSController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private LibraryItemFileHandler fileHandler = new();
        private LibraryManager _libraryManager;

        public LibraryController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            LibraryManager libraryManager) : base(mapper, unitOfWork, userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _libraryManager = libraryManager;
        }

        [HttpPost, DisableRequestSizeLimit]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                var fileName = Request.Form["myName"];
                var fileDescription = Request.Form["myDescription"];
                var result = _libraryManager.UploadFile(file, fileName, fileDescription);

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
                Debug.WriteLine(ex);
                return Ok();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetLibraryItems()
        {
            try
            {
                var items = await _libraryManager.GetItems();
                return Json(items);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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
                Debug.WriteLine(ex);
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
    }
}
