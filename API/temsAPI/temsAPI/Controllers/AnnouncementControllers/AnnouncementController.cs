using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.Announcement;

namespace temsAPI.Controllers.AnnouncementControllers
{
    public class AnnouncementController : TEMSController
    {
        private AnnouncementManager _announcementManager;
        public AnnouncementController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            AnnouncementManager announcementManager,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _announcementManager = announcementManager;
        }

        [HttpPost("announcement/Create")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ANNOUNCEMENTS)]
        [DefaultExceptionHandler("An error occured while creating the announcement")]
        public async Task<IActionResult> Create([FromBody] AddAnnouncementViewModel viewModel)
        {
            string result = await _announcementManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("announcement/Remove/{announcementId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ANNOUNCEMENTS)]
        [DefaultExceptionHandler("An error occured while removing the announcement")]
        public async Task<IActionResult> Remove(string announcementId)
        {
            string result = await _announcementManager.Remove(announcementId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("announcement/Get/{skip?}/{take?}")]
        [DefaultExceptionHandler("An error occured while fetching announcements")]
        public async Task<IActionResult> Get(int skip = 0, int take = int.MaxValue)
        {
            var announcements = await _announcementManager.GetAnnouncements(skip, take);
            return Ok(announcements);
        }
    }
}
