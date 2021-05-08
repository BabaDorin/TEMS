using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SIC_Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Services.SICServices;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Equipment;
using temsAPI.ViewModels.EquipmentType;
using temsAPI.ViewModels.Property;

namespace temsAPI.Controllers.EquipmentControllers
{
    public class EquipmentController : TEMSController
    {
        private EquipmentManager _equipmentManager;

        public EquipmentController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentManager equipmentManager)
           : base(mapper, unitOfWork, userManager)
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
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while saving equipment data", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddEquipmentViewModel viewModel)
        {
            try
            {
                string result = await _equipmentManager.Update(viewModel);
                if (result == null)
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
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
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
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplified(string id)
        {
            try
            {
                var equipment = await _equipmentManager.GetEquipmentById(id);
                if (equipment == null)
                    return ReturnResponse("Invalid equipment Id", ResponseStatus.Fail);

                var viewModel = _equipmentManager.EquipmentToViewEquipmentSimplifiedViewModel(equipment);
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
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetById(string id)
        {
            try
            {
                Equipment model = (await _unitOfWork.Equipments
                    .Find<Equipment>(
                        where: q => q.Id == id,
                        include: q => q
                        .Include(q => q.EquipmentDefinition).ThenInclude(q => q.Children)
                        .Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentSpecifications)
                        .ThenInclude(q => q.Property)
                        .Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentType)
                        .Include(q => q.EquipmentAllocations).ThenInclude(q => q.Room)
                        .Include(q => q.EquipmentAllocations).ThenInclude(q => q.Personnel)
                        .Include(q => q.Children)
                        .ThenInclude(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.EquipmentType)
                        .ThenInclude(q => q.Properties.Where(q => !q.IsArchieved))
                        .Include(q => q.Parent)
                        .ThenInclude(q => q.EquipmentDefinition)
                      )).FirstOrDefault();

                // Invalid id provided
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
        public async Task<JsonResult> GetEquipmentOfDefinitions(string[] definitionIds, bool onlyDeatachedEquipment = false)
        {
            try
            {
                if (definitionIds == null)
                    return ReturnResponse("Please, provide some definitions", ResponseStatus.Fail);

                Expression<Func<Equipment, bool>> expression = q =>
                    definitionIds.Contains(q.EquipmentDefinitionID) && !q.IsArchieved;

                if (onlyDeatachedEquipment)
                    expression = ExpressionCombiner.CombineTwo(expression, q => q.ParentID == null);

                var viewModel = (await _unitOfWork.Equipments
                    .Find<Option>(
                        include: q => q.Include(q => q.EquipmentDefinition),
                        where: expression,
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Identifier,
                            Additional = $"{q.Description} {q.EquipmentDefinition.Identifier}"
                        }
                    )).ToList();

                return Json(viewModel);
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
                var viewModel = (await _unitOfWork.Equipments
                    .Find<AddEquipmentViewModel>(
                        where: q => q.Id == equipmentId,
                        include: q => q
                        .Include(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.EquipmentType),
                        select: q => _mapper.Map<AddEquipmentViewModel>(q)
                      )).FirstOrDefault();

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
                string archievingResult = await (new ArchieveHelper(_userManager, _unitOfWork))
                    .SetEquipmentArchivationStatus(equipmentId, archivationStatus);

                if (archievingResult != null)
                    return ReturnResponse(archievingResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/detach/{childId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Detach(string childId)
        {
            try
            {
                var equipment = (await _unitOfWork.Equipments
                    .Find<Equipment>(q => q.Id == childId))
                    .FirstOrDefault();

                if (equipment == null)
                    return ReturnResponse("Invalid child ID provided.", ResponseStatus.Fail);

                equipment.ParentID = null;
                await _unitOfWork.Save();
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

                var parent = (await _unitOfWork.Equipments
                    .Find<Equipment>(q => q.Id == viewModel.ParentId))
                    .FirstOrDefault();

                foreach(var childId in viewModel.ChildrenIds)
                {
                    parent.Children.Add((await _unitOfWork.Equipments
                        .Find<Equipment>(q => q.Id == childId))
                        .FirstOrDefault());
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

        [HttpGet("equipment/changeworkingstate/{attribute}/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> ChangeWorkingState(string attribute, string equipmentId)
        {
            try
            {
                var model = (await _unitOfWork.Equipments
                    .Find<Equipment>(q => q.Id == equipmentId))
                    .FirstOrDefault();

                if (model == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                if(attribute.ToLower() == "isdefect")
                    model.IsDefect = !model.IsDefect;

                if (attribute.ToLower() == "isused")
                    model.IsUsed = !model.IsUsed;

                await _unitOfWork.Save();
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while change the equipment's work state", ResponseStatus.Fail);
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
        // -------------------------< Extract then to a separate file >--------------------------------
        private static ViewEquipmentSimplifiedViewModel EquipmentToViewEquipmentSimplifiedViewModel(Equipment q)
        {
            return new ViewEquipmentSimplifiedViewModel
            {
                Id = q.Id,
                IsDefect = q.IsDefect,
                IsUsed = q.IsUsed,
                TemsId = q.TEMSID,
                SerialNumber = q.SerialNumber,
                Type = q.EquipmentDefinition.EquipmentType.Name,
                Definition = q.EquipmentDefinition.Identifier,
                Assignee = (q.EquipmentAllocations.Count(q => q.DateReturned == null) > 0)
                                ? q.EquipmentAllocations.First(q => q.DateReturned == null).Assignee
                                : "Deposit",
                TemsIdOrSerialNumber = String.IsNullOrEmpty(q.TEMSID)
                                ? q.SerialNumber
                                : q.TEMSID
            };
        }
    }
}
