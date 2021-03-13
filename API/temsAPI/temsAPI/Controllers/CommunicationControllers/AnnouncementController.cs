using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.CommunicationEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Announcement;

namespace temsAPI.Controllers.CommunicationControllers
{
    public class AnnouncementController : TEMSController
    {
        public AnnouncementController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddAnnouncementViewModel viewModel)
        {

            // test this
            try
            {
                // Invalid data provided
                if (String.IsNullOrEmpty(viewModel.Title = viewModel.Title?.Trim()) ||
                    String.IsNullOrEmpty(viewModel.Text = viewModel.Text?.Trim()))
                    return ReturnResponse("Title and Text is required", ResponseStatus.Fail);

                Announcement model = new Announcement
                {
                    Id = Guid.NewGuid().ToString(),
                    DateCreated = DateTime.Now,
                    Message = viewModel.Text,
                    Title = viewModel.Title
                };

                await _unitOfWork.Announcements.Create(model);
                await _unitOfWork.Save();

                if (!await _unitOfWork.Announcements.isExists(q => q.Id == model.Id))
                    return ReturnResponse("Fail", ResponseStatus.Fail);
                else
                    return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the announcement", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            try
            {
                List<ViewAnnouncementViewModel> viewModel = (await _unitOfWork.Announcements
                    .FindAll<ViewAnnouncementViewModel>(
                        where: q => !q.IsArchieved,
                        select: q => new ViewAnnouncementViewModel
                        {
                            Id = q.Id,
                            Text = q.Message,
                            Title = q.Title,
                            DateCreated = q.DateCreated
                        }
                    )).OrderBy(q => q.DateCreated).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching announcements", ResponseStatus.Fail);
            }
        }
    }
}
