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
        ArchieveManager _archieveManager;

        public ArchieveController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            ArchieveManager archieveManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _archieveManager = archieveManager;
        }
        
        [HttpGet("/archieve/GetArchievedItems/{itemType}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetArchievedItems(string itemType)
        {
            try
            {
                var items = await _archieveManager.GetArchievedItems(itemType);
                return Ok(items);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while retrieveing archieved items", ResponseStatus.Fail);
            }
        }

        [HttpGet("archieve/SetArchivationStatus/{itemType}/{itemId}/{status}")]
        public async Task<IActionResult> SetArchivationStatus(string itemType, string itemId, bool status)
        {
            try
            {
                string archivationStatus = await _archieveManager.SetArchivationStatus(
                    itemType,
                    itemId,
                    status);
                if (archivationStatus != null)
                    return ReturnResponse(archivationStatus, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return ReturnResponse("An error occured while setting the archivation status", ResponseStatus.Fail);
            }
        }   
    }
}
