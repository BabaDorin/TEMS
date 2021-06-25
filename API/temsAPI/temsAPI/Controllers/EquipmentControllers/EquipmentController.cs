using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Services.SICServices;
using temsAPI.System_Files;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Controllers.EquipmentControllers
{
    public class EquipmentController : TEMSController
    {
        private EquipmentManager _equipmentManager;
        public EquipmentController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            ILogger<TEMSController> logger,
            EquipmentManager equipmentManager)
           : base(mapper, unitOfWork, userManager, logger)
        {
            _equipmentManager = equipmentManager;
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Add([FromBody] AddEquipmentViewModel viewModel)
        {
            try
            {
                string result = await _equipmentManager.Create(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                //_logger.Log(LogLevel.Error, ex, this.GetType().Name + " - " + new StackTrace().GetFrame(1).GetMethod().Name);
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while saving equipment data", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/remove/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> Remove(string equipmentId)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while removing the equipment", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddEquipmentViewModel viewModel)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while updating equipment data.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/getsimplified/{pageNumber}/{equipmentsPerPage}/{onlyParents}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetSimplified(
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
                    return Json(await _equipmentManager.GetEquipmentOfEntities(rooms, personnel));

                return Json(await _equipmentManager.GetEquipment(onlyParents));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("Unknown error occured when fetching equipments", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/getsimplified/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetSimplified(string id)
        {
            try
            {
                var equipment = await _equipmentManager.GetFullEquipmentById(id);
                if (equipment == null)
                    return ReturnResponse("Invalid equipment Id", ResponseStatus.Fail);

                var viewModel = ViewEquipmentSimplifiedViewModel.FromEquipment(equipment);
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An unhandled error occured when fetching equipment", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/getallautocompleteoptions/{onlyParents}/{filter?}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(bool onlyParents, string filter = null)
        {
            try
            {
                var viewModel = await _equipmentManager.GetAutocompleteOptions(onlyParents, filter);
                return Json(viewModel);
            }
            catch (Exception)
            {
                return ReturnResponse("An error occured when fetching autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/getbyid/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetById(string id)
        {
            try
            {
                var model = await _equipmentManager.GetFullEquipmentById(id);
                if (model == null)
                    return ReturnResponse("Invalid equipment id provided", ResponseStatus.Fail);

                var viewModel = ViewEquipmentViewModel.ParseEquipment(_mapper, model);

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching equipment", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/getequipmentofdefinitions")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetEquipmentOfDefinitions(List<string> definitionIds, bool onlyParents = false)
        {
            try
            {
                if (definitionIds == null)
                    return ReturnResponse("Please, provide some definitions", ResponseStatus.Fail);

                var options = await _equipmentManager.GetEquipmentOfDefinitions(definitionIds, onlyParents);
                return Json(options);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching equipment of definitions", ResponseStatus.Fail);
                throw;
            }
        }

        [HttpGet("equipment/getequipmenttoupdate/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetEquipmentToUpdate(string equipmentId)
        {
            try
            {
                var equipment = await _equipmentManager.GetFullEquipmentById(equipmentId);
                var viewModel = AddEquipmentViewModel.FromModel(equipment);

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching equipment data.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/archieve/{equipmentId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string equipmentId, bool archivationStatus = true)
        {
            try
            {
                string archivationResult = await (new ArchieveHelper(_unitOfWork, User))
                    .SetEquipmentArchivationStatus(equipmentId, archivationStatus);

                if (archivationResult != null)
                    return ReturnResponse(archivationResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/detach/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Detach(string equipmentId)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while detaching the child equipment.", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Attach([FromBody] AttachEquipmentViewModel viewModel)
        {
            try
            {
                string validationResult = await viewModel.Validate(_unitOfWork);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var parent = await _equipmentManager.GetById(viewModel.ParentId);
                foreach(var childId in viewModel.ChildrenIds)
                {
                    var child = await _equipmentManager.GetById(childId);
                    _equipmentManager.Attach(parent, child);
                }

                await _unitOfWork.Save();
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while attaching child equipment", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/changeworkingstate/{equipmentId}/{isWorking?}")]
        public async Task<JsonResult> ChangeWorkingState(string equipmentId, bool? isWorking)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while changing equipment working state", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/changeusingstate/{equipmentId}/{isUsed?}")]
        public async Task<JsonResult> ChangeUsingState(string equipmentId, bool? isUsed)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while setting equipment's using state", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> BulkUpload()
        {
            try
            {
                var files = Request.Form.Files;
                var bulkUploadResult = await new SICService(_unitOfWork).ValidateAndRegisterComputers(files);

                return Json(bulkUploadResult);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while uploading files", ResponseStatus.Fail);
            }
        }
    }
}
