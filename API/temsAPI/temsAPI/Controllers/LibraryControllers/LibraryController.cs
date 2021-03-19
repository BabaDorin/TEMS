using Aspose.Zip;
using Aspose.Zip.Saving;
using AutoMapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.LibraryEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.ViewModels.Library;

namespace temsAPI.Controllers.LibraryControllers
{
    public class LibraryController : TEMSController
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public LibraryController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            IHttpContextAccessor httpContextAccessor) : base(mapper, unitOfWork, userManager)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                if (file == null)
                {
                    Debug.WriteLine("file null");
                    throw new Exception("Null file ??");
                }


                AddLibraryItemViewModel viewModel = new AddLibraryItemViewModel
                {
                    DisplayName = Request.Form["myName"],
                    Description = Request.Form["myDescription"],
                    ActualName = FileUploadService.GetSanitarizedUniqueActualName(file),
                };

                string dbPath = FileUploadService.CompressAndSave(file, viewModel.ActualName);

                if(dbPath == null)
                    return StatusCode(500, $"File could not be uploaded");

                LibraryItem model = new LibraryItem
                {
                    Id = Guid.NewGuid().ToString(),
                    ActualName = viewModel.ActualName,
                    DateUploaded = DateTime.Now,
                    DbPath = dbPath,
                    Description = viewModel.Description,
                    DisplayName = viewModel.DisplayName,
                    FileSize = file.Length,
                };

                await _unitOfWork.LibraryItems.Create(model);
                await _unitOfWork.Save();

                if (await _unitOfWork.LibraryItems.isExists(q => q.Id == model.Id))
                    return Ok();

                // File has been saved to disk, but without any reference in db. Let's
                // delete the file since we ca'nt access it.
                await Task.Run(() =>
                {
                    FileUploadService.DeleteFile(model.DbPath);
                }).ConfigureAwait(false);
                return StatusCode(500, $"An error occured when saving the file in database." +
                    $" Consider uploading again.");
            }
            catch (ConnectionResetException ex)
            {
                // When the Client - Server connection has been "closed" (Where closed means that
                // the client stopped sending requests aka When TEMS Tab is closed or user canceled the upload,
                // this Exeption will be trown.

                // This behaviour helps us to easily implement the "cancel upload" feature (kind of), without
                // Third party Upload Controllers that cost money ;(
                Debug.WriteLine("----------------------------");
                Debug.WriteLine(ex);
                Debug.WriteLine("----------------------------");
                return Ok();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("----------------------------");
                Debug.WriteLine(ex);
                Debug.WriteLine("----------------------------");
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            try
            {
                // stuff
                return ReturnResponse("Canceled", ResponseStatus.Fail);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

    }
}
