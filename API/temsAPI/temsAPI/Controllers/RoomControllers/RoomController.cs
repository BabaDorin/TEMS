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
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels.Room;

namespace temsAPI.Controllers.RoomControllers
{
    public class RoomController : TEMSController
    {
        private RoomManager _roomManager;
        
        public RoomController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            RoomManager roomManager,
            ILogger<TEMSController> logger
            ) : base(mapper, unitOfWork, userManager, logger)
        {
            _roomManager = roomManager;
        }

        [HttpPost("room/Create")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Create([FromBody] AddRoomViewModel viewModel)
        {
            try
            {
                var result = await _roomManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when creating the room", ResponseStatus.Fail);
            }
        }

        [HttpPut("room/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Update([FromBody] AddRoomViewModel viewModel)
        {
            try
            {
                var result = await _roomManager.Update(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while saving the room", ResponseStatus.Fail);
            }
        }

        [HttpGet("room/Archieve/{roomId}/{archivationStatus?}")]
        public async Task<IActionResult> Archieve(string roomId, bool archivationStatus = true)
        {
            try
            {
                var archievingResult = await (new ArchieveHelper(_unitOfWork, User))
                    .SetRoomArchivationStatus(roomId, archivationStatus);
                if (archievingResult != null)
                    return ReturnResponse(archievingResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }

        [HttpDelete("room/Remove/{roomId}")]
        public async Task<IActionResult> Remove(string roomId)
        {
            try
            {
                string result = await _roomManager.Remove(roomId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the room", ResponseStatus.Fail);
            }
        }

        [HttpGet("room/GetAllAutocompleteOptions/{filter?}")]
        public async Task<IActionResult> GetAllAutocompleteOptions(string filter)
        {
            try
            {
                var options = await _roomManager.GetAutocompleteOptions(filter);
                return Ok(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpGet("room/GetRoomToUpdate/{roomId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetRoomToUpdate(string roomId)
        {
            try
            {
                var room = await _roomManager.GetById(roomId);
                var viewModel = AddRoomViewModel.FromModel(room);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while getting room data", ResponseStatus.Fail);
            }
        }

        [HttpGet("room/GetSimplified/{pageNumber}/{recordsPerPage}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetSimplified(int pageNumber, int recordsPerPage)
        {
            try
            {
                // Invalid page number or records per page provided
                if (pageNumber < 0 || recordsPerPage < 0 || pageNumber > 1000 || recordsPerPage > 1000)
                    return ReturnResponse("Invalid parameters, please provide real number representing page number" +
                        "and how many records are displayed per page", ResponseStatus.Fail);

                var rooms = await _roomManager.GetRoomsSimplified();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching rooms", ResponseStatus.Fail);
            }
        }

        [HttpGet("room/GetLabels")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetLabels()
        {
            try
            {
                var options = await _roomManager.GetLabelOptions();
                return Ok(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching room labels", ResponseStatus.Fail);
            }
        }

        [HttpGet("room/GetById/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var room = await _roomManager.GetFullById(id);
                if (room == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                var viewModel = ViewRoomViewModel.FromModel(room);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching the room", ResponseStatus.Fail);
            }
        }
    }
}
