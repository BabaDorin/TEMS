using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Helpers.Filters;
using temsAPI.Services.SICServices;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Controllers.EquipmentControllers
{
    public class EquipmentController : TEMSController
    {
        readonly EquipmentManager _equipmentManager;
        readonly LogManager _logManager;
        readonly SICService _sicService;

        public EquipmentController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            ILogger<TEMSController> logger,
            EquipmentManager equipmentManager,
            LogManager logManager,
            SICService sicService)
           : base(unitOfWork, userManager, logger)
        {
            _equipmentManager = equipmentManager;
            _logManager = logManager;
            _sicService = sicService;
        }

        [HttpPost("equipment/Add")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while saving equipment data")]
        public async Task<IActionResult> Add([FromBody] AddEquipmentViewModel viewModel)
        {
            string result = await _equipmentManager.Create(viewModel);
            if (result!= null)
                return ReturnResponse(result, ResponseStatus.Neutral);
            
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost("equipment/BulkUpload")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while uploading files. Make sure SIC has been integrated within your system.")]
        public async Task<IActionResult> BulkUpload()
        {
            var files = Request.Form.Files;
            var bulkUploadResult = await _sicService.ValidateAndRegisterComputers(files);

            return Ok(bulkUploadResult);
        }

        [HttpPut("equipment/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while updating equipment data.")]
        public async Task<IActionResult> Update([FromBody] AddEquipmentViewModel viewModel)
        {
            string result = await _equipmentManager.Update(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success!", ResponseStatus.Success);
        }

        [HttpGet("equipment/Archieve/{equipmentId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while changing the archivation status.")]
        public async Task<IActionResult> Archieve(string equipmentId, bool archivationStatus = true)
        {
            string archivationResult = await (new ArchieveHelper(_unitOfWork, User, _logManager))
                     .SetEquipmentArchivationStatus(equipmentId, archivationStatus);

            if (archivationResult != null)
                return ReturnResponse(archivationResult, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [HttpDelete("equipment/Remove/{equipmentId}")]
        [DefaultExceptionHandler("An error occured while removing the equipment")]
        public async Task<IActionResult> Remove(string equipmentId)
        {
            string result = await _equipmentManager.Remove(equipmentId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost("equipment/GetSimplified")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("Unknown error occured while fetching equipment records.")]
        public async Task<IActionResult> GetSimplified([FromBody] EquipmentFilter filter)
        {
            if(!filter.Validate())
                return ReturnResponse("Invalid filter provided", ResponseStatus.Neutral);

            return Ok(await _equipmentManager.GetEquipment(filter));
        }

        [HttpGet("equipment/GetSimplified/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An unhandled error occured while fetching equipment")]
        public async Task<IActionResult> GetSimplified(string id)
        {
            var equipment = await _equipmentManager.GetFullEquipmentById(id);
            if (equipment == null)
                return ReturnResponse("Invalid equipment Id", ResponseStatus.Neutral);

            var viewModel = ViewEquipmentSimplifiedViewModel.FromModel(equipment);
            return Ok(viewModel);
        }

        [HttpGet("equipment/GetAllAutocompleteOptions/{onlyParents}/{filter?}")]
        [DefaultExceptionHandler("An error occured while fetching autocomplete options")]
        public async Task<IActionResult> GetAllAutocompleteOptions(bool onlyParents, string filter = null)
        {
            var viewModel = await _equipmentManager.GetAutocompleteOptions(onlyParents, filter);
            return Ok(viewModel);
        }

        [HttpGet("equipment/GetById/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching equipment")]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _equipmentManager.GetFullEquipmentById(id);
            if (model == null)
                return ReturnResponse("Invalid equipment id provided", ResponseStatus.Neutral);

            var viewModel = ViewEquipmentViewModel.FromModel(model);
            return Ok(viewModel);
        }

        [HttpGet("equipment/GetEquipmentOfDefinitions")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching equipment of definitions")]
        public async Task<IActionResult> GetEquipmentOfDefinitions(List<string> definitionIds, bool onlyParents = false)
        {
            if (definitionIds == null)
                return ReturnResponse("Please, provide some definitions", ResponseStatus.Neutral);

            var options = await _equipmentManager.GetEquipmentOfDefinitions(definitionIds, onlyParents);
            return Ok(options);
        }

        [HttpGet("equipment/GetEquipmentToUpdate/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching equipment data.")]
        public async Task<IActionResult> GetEquipmentToUpdate(string equipmentId)
        {
            var equipment = await _equipmentManager.GetFullEquipmentById(equipmentId);
            var viewModel = AddEquipmentViewModel.FromModel(equipment);

            return Ok(viewModel);
        }

        [HttpGet("equipment/Detach/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while detaching the child equipment.")]
        public async Task<IActionResult> Detach(string equipmentId)
        {
            var equipment = await _equipmentManager.GetById(equipmentId);
            if (equipment == null)
                return ReturnResponse("Invalid child ID provided.", ResponseStatus.Neutral);

            string result = await _equipmentManager.Detach(equipment);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost("equipment/Attach")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while attaching child equipment")]
        public async Task<IActionResult> Attach([FromBody] AttachEquipmentViewModel viewModel)
        {
            string validationResult = await viewModel.Validate(_unitOfWork);
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Neutral);

            var parent = await _equipmentManager.GetFullEquipmentById(viewModel.ParentId);
            foreach (var childId in viewModel.ChildrenIds)
            {
                var child = await _equipmentManager.GetFullEquipmentById(childId);
                await _equipmentManager.Attach(parent, child);
            }

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("equipment/ChangeWorkingState/{equipmentId}/{isWorking?}")]
        [DefaultExceptionHandler("An error occured while changing equipment working state")]
        public async Task<IActionResult> ChangeWorkingState(string equipmentId, bool? isWorking)
        {
            var equipment = await _equipmentManager.GetById(equipmentId);
            if (equipment == null)
                return ReturnResponse("Invalid equipment ID", ResponseStatus.Neutral);

            // by default it works like a toggler
            await _equipmentManager.ChangeWorkingState(equipment, isWorking ?? !equipment.IsDefect);
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("equipment/ChangeUsingState/{equipmentId}/{isUsed?}")]
        [DefaultExceptionHandler("An error occured while setting equipment's using state")]
        public async Task<IActionResult> ChangeUsingState(string equipmentId, bool? isUsed)
        {
            var equipment = (await _unitOfWork.Equipments
                    .Find<Data.Entities.EquipmentEntities.Equipment>(
                        where: q => q.Id == equipmentId,
                        include: q => q
                        .Include(q => q.Children)))
                    .FirstOrDefault();

            if (equipment == null)
                return ReturnResponse("Invalid equipment id provided", ResponseStatus.Neutral);

            // by default it works like a toggler
            await _equipmentManager.ChangeUsingState(equipment, isUsed ?? !equipment.IsUsed);
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("equipment/IsTEMSIDAvailable")]
        [DefaultExceptionHandler("An error occured while checking for TEMSID availability")]
        public async Task<IActionResult> ChangeUsingState([FromQuery] string temsid)
        {
            return Ok(await _equipmentManager.IsTEMSIDAvailable(temsid));
        }

        [HttpGet("equipment/findIdByTEMSID")]
        [DefaultExceptionHandler("An error occured while searching for equipment's Id")]
        public async Task<IActionResult> FindIdByTEMSID([FromQuery] string temsId)
        {
            var id = await _equipmentManager.FindIdByTEMSID(temsId);
            if (id == null)
                return ReturnResponse("Invalid TEMSID provided", ResponseStatus.Neutral);

            return Ok(Json(id));
        }
    }
}
