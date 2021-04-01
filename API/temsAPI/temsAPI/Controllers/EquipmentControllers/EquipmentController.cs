﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
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
            string validationResult = await ValidateAddEquipmentViewModel(viewModel);
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Fail);

                // If we got so far, it might be valid
            Equipment model = _mapper.Map<Equipment>(viewModel);

            model.Id = Guid.NewGuid().ToString();

            await _unitOfWork.Equipments.Create(model);
            await _unitOfWork.Save();

            //if (!await _unitOfWork.Equipments.isExists(q => q.Id == model.Id))
            //    return ReturnResponse("Fail", ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddEquipmentViewModel viewModel)
        {
            try
            {
                string validationResult = await ValidateAddEquipmentViewModel(viewModel);
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
                        include: q => q.Include(q => q.EquipmentDefinition)
                        .ThenInclude(q => q.EquipmentType)
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

                var activeRoomAllocation = model.EquipmentAllocations
                    .Where(q => q.DateReturned == null && q.RoomID != null)
                    ?.FirstOrDefault();

                var activePersonnelAllocation = model.EquipmentAllocations
                    .Where(q => q.DateReturned == null && q.PersonnelID != null)
                    ?.FirstOrDefault();

                ViewEquipmentViewModel viewModel = new ViewEquipmentViewModel
                {
                    Id = model.Id,
                    Definition = new Option
                    {
                        Value = model.EquipmentDefinition.Id,
                        Label = model.EquipmentDefinition.Identifier,
                        Additional = model.EquipmentDefinition.Description
                    },
                    IsDefect = model.IsDefect,
                    IsUsed = model.IsUsed,
                    IsArchieved = model.IsArchieved,
                    SerialNumber = model.SerialNumber,
                    TemsId = model.TEMSID,
                    Type = model.EquipmentDefinition.EquipmentType.Name,
                    Personnnel = (activePersonnelAllocation == null)
                        ? null
                        : new Option
                        {
                            Value = activePersonnelAllocation.PersonnelID,
                            Label = activePersonnelAllocation.Personnel.Name
                        },
                    Room = (activeRoomAllocation == null)
                        ? new Option { Value = "Deposit", Label = "Deposit" }
                        : new Option
                        {
                            Value = activeRoomAllocation.RoomID,
                            Label = activeRoomAllocation.Room.Identifier
                        },
                    Parent = (model.Parent == null)
                        ? null
                        : new Option
                        {
                            Value = model.Parent.Id,
                            Label = model.Parent.EquipmentDefinition.Identifier,
                        },
                    Children = model.Children
                        .Select(q => new Option
                        {
                            Value = q.Id,
                            Label = q.EquipmentDefinition.Identifier,
                            Additional = q.EquipmentDefinition.EquipmentType.Name
                        }).ToList(),
                    SpecificTypeProperties = _mapper.Map<List<ViewPropertyViewModel>>
                            (model.EquipmentDefinition.EquipmentType.Properties),
                };

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching equipment", ResponseStatus.Fail);
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

        [HttpGet("equipment/archieve/{equipmentId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string equipmentId)
        {
            try
            {
                string archievingResult = await (new ArchieveHelper(_userManager, _unitOfWork))
                    .ArchieveEquipment(equipmentId);

                if (archievingResult != null)
                    return ReturnResponse(archievingResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while archieving the equipment", ResponseStatus.Fail);
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

        /// <summary>
        /// Validates an instance of AddEquipmentViewModel. Returns null if everything is ok, otherwise - returns
        /// an error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<string> ValidateAddEquipmentViewModel(AddEquipmentViewModel viewModel)
        {
            Equipment updateModel = null;

            // It's the update case and the provided id is invalid
            if (viewModel.Id != null)
            {
                updateModel = (await _unitOfWork.Equipments
                    .Find<Equipment>(q => q.Id == viewModel.Id)).FirstOrDefault();

                if (updateModel == null)
                    return "Invalid id provided";
            }

            // at least one (TEMSID or SerialNumber) should be provided
            viewModel.Temsid = viewModel.Temsid?.Trim();
            viewModel.SerialNumber = viewModel.SerialNumber?.Trim();
            if (String.IsNullOrEmpty(viewModel.Temsid) && String.IsNullOrEmpty(viewModel.SerialNumber))
                return "Please, provide information for TemsID and / or SerialNumber";

            // Equipment already exists and it's not the update case
            if(updateModel == null)
            {
                if (!String.IsNullOrEmpty(viewModel.Temsid) &&
                    await _unitOfWork.Equipments.isExists(q => q.TEMSID == viewModel.Temsid) ||
                    !String.IsNullOrEmpty(viewModel.SerialNumber) &&
                    await _unitOfWork.Equipments.isExists(q => q.SerialNumber == viewModel.SerialNumber))
                    return "An equipment with the same TEMSID or Serial number already exists.";
            }
            else
            {
                if (viewModel.Temsid != updateModel.TEMSID
                    && await _unitOfWork.Equipments.isExists(q => q.TEMSID == viewModel.Temsid))
                    return "An equipment with the specified TEMSID already exists";

                if (viewModel.SerialNumber!= updateModel.SerialNumber
                    && await _unitOfWork.Equipments.isExists(q => q.SerialNumber== viewModel.SerialNumber))
                    return "An equipment with the specified Serial number already exists";
            }

            // No value provided for purchase date
            if (viewModel.PurchaseDate == new DateTime())
                viewModel.PurchaseDate = DateTime.Now;

            // Invalid price data
            if (viewModel.Price < 0 ||
                (new List<string> { "lei", "eur", "usd" }).IndexOf(viewModel.Currency) == -1)
                return "Invalid price data provided.";

            // Invalid definition provided
            // Case 1: Invalid id
            if (!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == viewModel.EquipmentDefinitionID))
                return "An equipment definition having the specified id has not been found.";

            // Case 2: It's the update case and the new definition is different from the old one
            if (updateModel != null && viewModel.EquipmentDefinitionID != updateModel.EquipmentDefinitionID)
                return "The new equipment definition should match the old one.";

            return null;
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
