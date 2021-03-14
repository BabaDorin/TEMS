using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Controllers.LibraryControllers
{
    public class LibraryController : TEMSController
    {
        private IWebHostEnvironment _hostingEnvironment;

        public LibraryController(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            UserManager<TEMSUser> userManager,
            IWebHostEnvironment hostingEnvironment) : base(mapper, unitOfWork, userManager)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private Task upload = null;

        [HttpPost, DisableRequestSizeLimit]
        public async Task<JsonResult> UploadFile()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                if (file.Length == 0)
                    return ReturnResponse($"No file provided,", ResponseStatus.Fail);

                var folderName = Path.Combine("StaticFiles", "LibraryUploads");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                    file.CopyTo(stream);

                return ReturnResponse($"{fileName} has been successfuly uploaded", ResponseStatus.Success);
            }
            catch (COMException ex)
            {
                Console.WriteLine("-------------------------File upload canceled----------------------");
                Debug.WriteLine(ex);
                Console.WriteLine("--------------------------------------------------------------");
                return ReturnResponse($"Upload canceled", ResponseStatus.Fail);
            }
            catch(Exception ex)
            {
                Console.WriteLine("-------------------------File upload failed----------------------");
                Debug.WriteLine(ex);
                Console.WriteLine("--------------------------------------------------------------");

                return ReturnResponse($"Upload failed", ResponseStatus.Fail);
            }
        }
    }
}
