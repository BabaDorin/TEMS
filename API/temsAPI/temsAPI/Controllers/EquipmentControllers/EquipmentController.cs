using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Services.SICServices;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.System_Files.TEMSFileLogger;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Controllers.EquipmentControllers
{
    public class EquipmentController : TEMSController
    {
        private EquipmentManager _equipmentManager;
        private LogManager _logManager;

        public EquipmentController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            ILogger<TEMSController> logger,
            EquipmentManager equipmentManager,
            LogManager logManager)
           : base(mapper, unitOfWork, userManager, logger)
        {
            _equipmentManager = equipmentManager;
            _logManager = logManager;
        }

        [HttpPost("equipment/Add")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while saving equipment data")]
        public async Task<IActionResult> Add([FromBody] AddEquipmentViewModel viewModel)
        {
            string result = await _equipmentManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);
            
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost("equipment/BulkUpload")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> BulkUpload()
        {
            try
            {
                var files = Request.Form.Files;
                var bulkUploadResult = await new SICService(_unitOfWork).ValidateAndRegisterComputers(files);

                return Ok(bulkUploadResult);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while uploading files. Make sure SIC has been integrated within your system.", ResponseStatus.Fail);
            }
        }

        [HttpPut("equipment/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Update([FromBody] AddEquipmentViewModel viewModel)
        {
            try
            {
                string result = await _equipmentManager.Update(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while updating equipment data.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/Archieve/{equipmentId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Archieve(string equipmentId, bool archivationStatus = true)
        {
            try
            {
                string archivationResult = await (new ArchieveHelper(_unitOfWork, User, _logManager))
                    .SetEquipmentArchivationStatus(equipmentId, archivationStatus);

                if (archivationResult != null)
                    return ReturnResponse(archivationResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }

        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [HttpDelete("equipment/Remove/{equipmentId}")]
        public async Task<IActionResult> Remove(string equipmentId)
        {
            try
            {
                string result = await _equipmentManager.Remove(equipmentId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the equipment", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/GetSimplified/{pageNumber}/{equipmentsPerPage}/{onlyParents}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetSimplified(
            int pageNumber, 
            int equipmentsPerPage, 
            bool onlyParents,
            List<string> rooms,
            List<string> personnel)
        {
            try
            {
                // Invalid parameters
                if (pageNumber < 0 || equipmentsPerPage < 1)
                    return ReturnResponse("Invalid parameters", ResponseStatus.Fail);

                if (rooms != null && rooms.Count > 0 || personnel != null && personnel.Count > 0)
                    return Ok(await _equipmentManager.GetEquipmentOfEntities(rooms, personnel));

                return Ok(await _equipmentManager.GetEquipment(onlyParents));
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("Unknown error occured while fetching equipment records.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/GetSimplified/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetSimplified(string id)
        {
            try
            {
                var equipment = await _equipmentManager.GetFullEquipmentById(id);
                if (equipment == null)
                    return ReturnResponse("Invalid equipment Id", ResponseStatus.Fail);

                var viewModel = ViewEquipmentSimplifiedViewModel.FromEquipment(equipment);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An unhandled error occured when fetching equipment", ResponseStatus.Fail);
            }
        }

        [DefaultExceptionHandler("An error occured while fetching autocomplete options")]
        [HttpGet("equipment/GetAllAutocompleteOptions/{onlyParents}/{filter?}")]
        public async Task<IActionResult> GetAllAutocompleteOptions(bool onlyParents, string filter = null)
        {
            var viewModel = await _equipmentManager.GetAutocompleteOptions(onlyParents, filter);
            throw new ArgumentException("cf uratule");
            return Ok(viewModel);
        }

        [HttpGet("equipment/GetById/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var model = await _equipmentManager.GetFullEquipmentById(id);
                if (model == null)
                    return ReturnResponse("Invalid equipment id provided", ResponseStatus.Fail);

                var viewModel = ViewEquipmentViewModel.ParseEquipment(_mapper, model);

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching equipment", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/GetEquipmentOfDefinitions")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetEquipmentOfDefinitions(List<string> definitionIds, bool onlyParents = false)
        {
            try
            {
                if (definitionIds == null)
                    return ReturnResponse("Please, provide some definitions", ResponseStatus.Fail);

                var options = await _equipmentManager.GetEquipmentOfDefinitions(definitionIds, onlyParents);
                return Ok(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching equipment of definitions", ResponseStatus.Fail);
                throw;
            }
        }

        [HttpGet("equipment/GetEquipmentToUpdate/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetEquipmentToUpdate(string equipmentId)
        {
            try
            {
                var equipment = await _equipmentManager.GetFullEquipmentById(equipmentId);
                var viewModel = AddEquipmentViewModel.FromModel(equipment);

                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching equipment data.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/Detach/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Detach(string equipmentId)
        {
            try
            {
                var equipment = await _equipmentManager.GetById(equipmentId);
                if (equipment == null)
                    return ReturnResponse("Invalid child ID provided.", ResponseStatus.Fail);

                string result = await _equipmentManager.DetachEquipment(equipment);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while detaching the child equipment.", ResponseStatus.Fail);
            }
        }

        [HttpPost("equipment/Attach")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Attach([FromBody] AttachEquipmentViewModel viewModel)
        {
            try
            {
                string validationResult = await viewModel.Validate(_unitOfWork);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var parent = await _equipmentManager.GetFullEquipmentById(viewModel.ParentId);
                foreach(var childId in viewModel.ChildrenIds)
                {
                    var child = await _equipmentManager.GetFullEquipmentById(childId);
                    await _equipmentManager.Attach(parent, child);
                }

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while attaching child equipment", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/ChangeWorkingState/{equipmentId}/{isWorking?}")]
        public async Task<IActionResult> ChangeWorkingState(string equipmentId, bool? isWorking)
        {
            try
            {
                var equipment = await _equipmentManager.GetById(equipmentId);
                if (equipment == null)
                    return ReturnResponse("Invalid equipment ID", ResponseStatus.Fail);

                // by default it works like a toggler
                await _equipmentManager.ChangeWorkingState(equipment, isWorking ?? !equipment.IsDefect);
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while changing equipment working state", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/ChangeUsingState/{equipmentId}/{isUsed?}")]
        public async Task<IActionResult> ChangeUsingState(string equipmentId, bool? isUsed)
        {
            try
            {
                var equipment = (await _unitOfWork.Equipments
                    .Find<Data.Entities.EquipmentEntities.Equipment>(
                        where: q => q.Id == equipmentId,
                        include: q => q
                        .Include(q => q.Children)))
                    .FirstOrDefault();

                if (equipment == null)
                    return ReturnResponse("Invalid equipment id provided", ResponseStatus.Fail);

                // by default it works like a toggler
                await _equipmentManager.ChangeUsingState(equipment, isUsed ?? !equipment.IsUsed);
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while setting equipment's using state", ResponseStatus.Fail);
            }
        }
    }
}
