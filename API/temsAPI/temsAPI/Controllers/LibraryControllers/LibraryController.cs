using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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
        public void UploadFile()
        {
            try
            {
                IFormFile file = null;

                    file = Request.Form.Files[0];

                    if (file == null)
                    {
                        Debug.WriteLine("file null");
                        throw new Exception();
                    }

                    var folderName = Path.Combine("StaticFiles", "LibraryUploads");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        var fullPath = Path.Combine(pathToSave, fileName);
                        var dbPath = Path.Combine(folderName, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                    }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            try
            {
                return StatusCode(200, $"nice");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

    }
}
