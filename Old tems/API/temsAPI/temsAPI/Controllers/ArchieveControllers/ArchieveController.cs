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

namespace temsAPI.Controllers.ArchieveControllers
{
    public class ArchieveController : TEMSController
    {
        readonly ArchieveManager _archieveManager;

        public ArchieveController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            ArchieveManager archieveManager,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _archieveManager = archieveManager;
        }
        
        [HttpGet("/archieve/GetArchievedItems/{itemType}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching archieved items")]
        public async Task<IActionResult> GetArchievedItems(string itemType)
        {
            var items = await _archieveManager.GetArchievedItems(itemType);
            return Ok(items);
        }

        [HttpGet("archieve/SetArchivationStatus/{itemType}/{itemId}/{status}")]
        [DefaultExceptionHandler("An error occured while setting the archivation status")]
        public async Task<IActionResult> SetArchivationStatus(string itemType, string itemId, bool status)
        {
            string archivationStatus = await _archieveManager.SetArchivationStatus(
                    itemType,
                    itemId,
                    status);
            if (archivationStatus != null)
                return ReturnResponse(archivationStatus, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }   
    }
}
