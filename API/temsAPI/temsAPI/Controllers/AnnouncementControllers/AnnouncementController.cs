﻿using AutoMapper;
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
using temsAPI.Data.Managers;
using temsAPI.System_Files;
using temsAPI.ViewModels;
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

        [HttpGet]
        public async Task<JsonResult> Get()
        {
            try
            {
                var announcements = await _announcementManager.GetAnnouncements();
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
