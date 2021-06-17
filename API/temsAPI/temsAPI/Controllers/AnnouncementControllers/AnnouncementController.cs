﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files;
using temsAPI.ViewModels.Announcement;

namespace temsAPI.Controllers.AnnouncementControllers
{
    public class AnnouncementController : TEMSController
    {
        private AnnouncementManager _announcementManager;
        public AnnouncementController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            AnnouncementManager announcementManager) : base(mapper, unitOfWork, userManager)
        {
            _announcementManager = announcementManager;
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ANNOUNCEMENTS)]
        public async Task<JsonResult> Create([FromBody] AddAnnouncementViewModel viewModel)
        {
            try
            {
                string result = await _announcementManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the announcement", ResponseStatus.Fail);
            }
        }

        [HttpGet("announcement/get/{skip?}/{take?}")]
        public async Task<JsonResult> Get(int skip = 0, int take = int.MaxValue)
        {
            try
            {
                var announcements = await _announcementManager.GetAnnouncements(skip, take);
                return Json(announcements);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching announcements", ResponseStatus.Fail);
            }
        }
    }
}
