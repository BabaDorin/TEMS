using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
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
        [DefaultExceptionHandler("An error occured when creating the room")]
        public async Task<IActionResult> Create([FromBody] AddRoomViewModel viewModel)
        {
            var result = await _roomManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success!", ResponseStatus.Success);
        }

        [HttpPut("room/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while saving the room")]
        public async Task<IActionResult> Update([FromBody] AddRoomViewModel viewModel)
        {
            var result = await _roomManager.Update(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("room/Archieve/{roomId}/{archivationStatus?}")]
        [DefaultExceptionHandler("An error occured while changing the archivation status")]
        public async Task<IActionResult> Archieve(string roomId, bool archivationStatus = true)
        {
            var archievingResult = await (new ArchieveHelper(_unitOfWork, User))
                    .SetRoomArchivationStatus(roomId, archivationStatus);
            if (archievingResult != null)
                return ReturnResponse(archievingResult, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("room/Remove/{roomId}")]
        [DefaultExceptionHandler("An error occured while removing the room")]
        public async Task<IActionResult> Remove(string roomId)
        {
            string result = await _roomManager.Remove(roomId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("room/GetAllAutocompleteOptions/{filter?}")]
        [DefaultExceptionHandler("An error occured when fetching autocomplete options")]
        public async Task<IActionResult> GetAllAutocompleteOptions(string filter)
        {
            var options = await _roomManager.GetAutocompleteOptions(filter);
            return Ok(options);
        }

        [HttpGet("room/GetRoomToUpdate/{roomId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while getting room data")]
        public async Task<IActionResult> GetRoomToUpdate(string roomId)
        {
            var room = await _roomManager.GetById(roomId);
            var viewModel = AddRoomViewModel.FromModel(room);
            return Ok(viewModel);
        }

        [HttpGet("room/GetSimplified/{pageNumber}/{recordsPerPage}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching rooms")]
        public async Task<IActionResult> GetSimplified(int pageNumber, int recordsPerPage)
        {
            // Invalid page number or records per page provided
            if (pageNumber < 0 || recordsPerPage < 0 || pageNumber > 1000 || recordsPerPage > 1000)
                return ReturnResponse("Invalid parameters, please provide real number representing page number" +
                    "and how many records are displayed per page", ResponseStatus.Neutral);

            var rooms = await _roomManager.GetRoomsSimplified();
            return Ok(rooms);
        }

        [HttpGet("room/GetLabels")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching room labels")]
        public async Task<IActionResult> GetLabels()
        {
            var options = await _roomManager.GetLabelOptions();
            return Ok(options);
        }

        [HttpGet("room/GetById/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching the room")]
        public async Task<IActionResult> GetById(string id)
        {
            var room = await _roomManager.GetFullById(id);
            if (room == null)
                return ReturnResponse("Invalid id provided", ResponseStatus.Neutral);

            var viewModel = ViewRoomViewModel.FromModel(room);
            return Ok(viewModel);
        }
    }
}
