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

        public EquipmentController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
           : base(mapper, unitOfWork, userManager)
        {

        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Add([FromBody] AddEquipmentViewModel viewModel)
        {
            try
            {
                string validationResult = await AddEquipmentViewModel.Validate(_unitOfWork, viewModel);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                // If we got so far, it might be valid
                var equipment = Equipment.FromViewModel(User, viewModel);
                await _unitOfWork.Equipments.Create(equipment);
                await _unitOfWork.Save();
                
                if (!await _unitOfWork.Equipments.isExists(q => q.Id == equipment.Id))
                    return ReturnResponse("Fail", ResponseStatus.Fail);

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
                string validationResult = await AddEquipmentViewModel.Validate(_unitOfWork, viewModel);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var model = (await _unitOfWork.Equipments
                    .Find<Equipment>(q => q.Id == viewModel.Id))
                    .FirstOrDefault();

                model.Currency = viewModel.Currency;
                model.Description = viewModel.Description;
                model.IsDefect = viewModel.IsDefect;
                model.IsUsed = viewModel.IsUsed;
                model.Price = viewModel.Price;
                model.PurchaseDate = viewModel.PurchaseDate;
                model.SerialNumber = viewModel.SerialNumber;
                model.TEMSID = viewModel.Temsid;

                await _unitOfWork.Save();

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
                DateTime start = DateTime.Now;
                // Invalid parameters
                if (pageNumber < 0 || equipmentsPerPage < 1)
                    return ReturnResponse("Invalid parameters", ResponseStatus.Fail);

                if (rooms != null && rooms.Count > 0 || personnel != null && personnel.Count > 0)
                    return Json(await GetEquipmentsOfEntities(rooms, personnel));

                Expression<Func<Equipment, bool>> expression
                    = (onlyParents) ? qu => qu.ParentID == null && !qu.IsArchieved : qu => !qu.IsArchieved;

                var viewModel = (await _unitOfWork.Equipments.FindAll<ViewEquipmentSimplifiedViewModel>
                        (expression,
                        include: q => q
                        .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                        .ThenInclude(q => q.Room)
                        .Include(q => q.EquipmentAllocations.Where(q => q.DateReturned == null))
                        .ThenInclude(q => q.Personnel)
                        .Include(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.EquipmentType),
                        select: q => EquipmentToViewEquipmentSimplifiedViewModel(q))).ToList();

                return Json(viewModel);
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
                // Invalid Id provied
                if (!await _unitOfWork.Equipments.isExists(q => q.Id == id))
                    return ReturnResponse("We could not find any equipment having the specified id", ResponseStatus.Fail);

                Equipment model = (await _unitOfWork.Equipments
                    .Find<Equipment>(
                    where: q => q.Id == id,
                    include: q => q
                                .Include(q => q.EquipmentDefinition)
                                .ThenInclude(q => q.EquipmentType)))
                    .FirstOrDefault();

                return Json(await EquipmentToEquipmentSimplifiedMapping(model));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An unhandled error occured when fetching equipment", ResponseStatus.Fail);
            }
        }

        [HttpGet("equipment/getallautocompleteoptions/{onlyParents}/{filter?}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(bool onlyParents, string? filter = null)
        {
            try
            {
                Expression<Func<Equipment, bool>> expression =
                   (onlyParents)
                   ? qu => qu.ParentID == null && !qu.IsArchieved
                   : qu => !qu.IsArchieved;

                if (filter != null)
                {
                    Expression<Func<Equipment, bool>> expression2 =
                        q => q.TEMSID.Contains(filter);

                    expression = ExpressionCombiner.CombineTwo(expression, expression2);
                }

                List<Option> autocompleteOptions = new List<Option>();

                var filteresEquipments = (await _unitOfWork.Equipments
                    .FindAll<Equipment>(
                        where: expression,
                        include: q => q.Include(q => q.EquipmentDefinition),
                        take: 5
                     ))
                    .ToList();

                filteresEquipments
                    .ForEach(q =>
                    {
                        if (!String.IsNullOrEmpty(q.TEMSID))
                            autocompleteOptions.Add(new Option
                            {
                                Value = q.Id,
                                Label = q.TEMSID,
                                Additional = q.EquipmentDefinition.Identifier
                            });

                        if (!String.IsNullOrEmpty(q.SerialNumber))
                            autocompleteOptions.Add(new Option
                            {
                                Value = q.Id,
                                Label = q.SerialNumber,
                                Additional = q.EquipmentDefinition.Identifier
                            });
                    });

                return Json(autocompleteOptions);
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
        private async Task<ViewEquipmentSimplifiedViewModel> EquipmentToEquipmentSimplifiedMapping(Equipment eq)
        {
            ViewEquipmentSimplifiedViewModel viewEquipmentSimplified = new ViewEquipmentSimplifiedViewModel
            {
                Id = eq.Id,
                IsDefect = eq.IsDefect,
                IsUsed = eq.IsUsed,
                IsArchieved = eq.IsArchieved,
                TemsId = eq.TEMSID,
                SerialNumber = eq.SerialNumber,
                Type = eq.EquipmentDefinition.EquipmentType.Name,
                Definition = eq.EquipmentDefinition.Identifier,
            };

            var lastAllocation = (await _unitOfWork.EquipmentAllocations
                .Find<EquipmentAllocation>(q => q.EquipmentID == eq.Id && q.DateReturned == null,
                include: q => q.Include(q => q.Room).Include(q => q.Personnel))).FirstOrDefault();

            if (lastAllocation == null)
                viewEquipmentSimplified.Assignee = "Deposit";
            else
                viewEquipmentSimplified.Assignee =
                    (lastAllocation.Room != null)
                    ? "Room: " + lastAllocation.Room.Identifier
                    : "Personnel: " + lastAllocation.Personnel.Name;

            viewEquipmentSimplified.TemsIdOrSerialNumber =
                String.IsNullOrEmpty(viewEquipmentSimplified.TemsId)
                ? viewEquipmentSimplified.SerialNumber
                : viewEquipmentSimplified.TemsId;

            return viewEquipmentSimplified;
        }

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

        private async Task<List<ViewEquipmentSimplifiedViewModel>> GetEquipmentsOfEntities(List<string> rooms, List<string> personnel)
        {
            Expression<Func<EquipmentAllocation, bool>> eqOfRoomsExpression = null;
            if (rooms.Count > 0)
                eqOfRoomsExpression = q => q.DateReturned == null && rooms.Contains(q.RoomID);

            Expression<Func<EquipmentAllocation, bool>> eqOfPersonnelExpression = null;
            if (personnel.Count > 0)
                eqOfPersonnelExpression = q => q.DateReturned == null && personnel.Contains(q.PersonnelID);

            Expression<Func<EquipmentAllocation, bool>> finalExpression =
                ExpressionCombiner.CombineTwo(eqOfRoomsExpression, eqOfPersonnelExpression);

            List<ViewEquipmentSimplifiedViewModel> viewModel = (await _unitOfWork.EquipmentAllocations
                .FindAll<ViewEquipmentSimplifiedViewModel>(
                    where: finalExpression,
                    include: q => q.Include(q => q.Equipment)
                    .ThenInclude(q => q.EquipmentDefinition)
                    .ThenInclude(q => q.EquipmentType)
                    .Include(q => q.Room)
                    .Include(q => q.Personnel),
                    select: q => EquipmentToViewEquipmentSimplifiedViewModel(q.Equipment)
                    )).ToList();

            return viewModel;
        } 
    }
}
