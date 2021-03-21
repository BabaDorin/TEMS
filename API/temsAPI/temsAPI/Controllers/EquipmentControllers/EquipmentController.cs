using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
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
        public async Task<JsonResult> Create([FromBody] AddEquipmentViewModel viewModel)
        {
            // one or both of TEMSID and SerialNumber properties should be given values
            if ((viewModel.Temsid == null ||
                String.IsNullOrEmpty(viewModel.Temsid = viewModel.Temsid.Trim())) &&
                (viewModel.SerialNumber == null ||
                String.IsNullOrEmpty(viewModel.SerialNumber = viewModel.SerialNumber.Trim())))
                return ReturnResponse("Please, provide information for TemsID and / or SerialNumber", ResponseStatus.Fail);

            // Equipment is already created
            if (!String.IsNullOrEmpty(viewModel.Temsid) &&
                await _unitOfWork.Equipments.isExists(q => q.TEMSID == viewModel.Temsid) ||
                !String.IsNullOrEmpty(viewModel.SerialNumber) &&
                await _unitOfWork.Equipments.isExists(q => q.SerialNumber == viewModel.SerialNumber))
                return ReturnResponse("This equipment already exists.", ResponseStatus.Fail);



            // Value for purchase date wasn't provided
            if (viewModel.PurchaseDate == new DateTime())
                viewModel.PurchaseDate = DateTime.Now;

            // Invalid price data
            if (viewModel.Price < 0 ||
                (new List<string> { "lei", "eur", "usd" }).IndexOf(viewModel.Currency) == -1)
                return ReturnResponse("Invalid price data provided.", ResponseStatus.Fail);

            // Invalid definition provided
            if (!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == viewModel.EquipmentDefinitionID))
                return ReturnResponse("An equipment definition having the specified id has not been found.", ResponseStatus.Fail);

            // If we got so far, it might be valid
            Data.Entities.EquipmentEntities.Equipment model =
                _mapper.Map<Data.Entities.EquipmentEntities.Equipment>(viewModel);

            model.Id = Guid.NewGuid().ToString();

            await _unitOfWork.Equipments.Create(model);
            await _unitOfWork.Save();

            if (!await _unitOfWork.Equipments.isExists(q => q.Id == model.Id))
                return ReturnResponse("Fail", ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("equipment/getsimplified/{pageNumber}/{equipmentsPerPage}/{onlyParents}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplified(int pageNumber, int equipmentsPerPage, bool onlyParents)
        {
            try
            {
                // Invalid parameters
                if (pageNumber < 0 || equipmentsPerPage < 1)
                    return ReturnResponse("Invalid parameters", ResponseStatus.Fail);


                Expression<Func<Data.Entities.EquipmentEntities.Equipment, bool>> expression
                    = (onlyParents) ? qu => qu.ParentID == null : null;

                IList<Data.Entities.EquipmentEntities.Equipment> equipments =
                    equipments = await _unitOfWork.Equipments.FindAll<Data.Entities.EquipmentEntities.Equipment>
                        (expression,
                        include: q => q
                                .Include(q => q.EquipmentDefinition)
                                .ThenInclude(q => q.EquipmentType));

                var viewModel = new List<ViewEquipmentSimplifiedViewModel>();

                foreach (Data.Entities.EquipmentEntities.Equipment eq in equipments)
                {
                    viewModel.Add(await EquipmentToEquipmentSimplifiedMapping(eq));
                }

                return Json(viewModel);
            }
            catch (Exception)
            {
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

        [HttpGet("equipment/getallautocompleteoptions/{onlyParents}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(bool onlyParents)
        {
            try
            {
                Expression<Func<Data.Entities.EquipmentEntities.Equipment, bool>> expression =
                   (onlyParents)
                   ? qu => qu.ParentID == null
                   : null;

                List<Option> autocompleteOptions = new List<Option>();

                (await _unitOfWork.Equipments
                    .FindAll<Data.Entities.EquipmentEntities.Equipment>(
                        where: expression,
                        include: q => q.Include(q => q.EquipmentDefinition)
                     ))
                    .ToList()
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
                // Invalid id provided
                if (!await _unitOfWork.Equipments.isExists(q => q.Id == id))
                    return ReturnResponse("Invalid equipment id provided", ResponseStatus.Fail);

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
                        .ThenInclude(q => q.Properties)
                        .Include(q => q.Parent)
                        .ThenInclude(q => q.EquipmentDefinition)
                      )).FirstOrDefault();

                
                ViewEquipmentViewModel viewModel = new ViewEquipmentViewModel
                {
                    Id = model.Id,
                    Identifier = model.EquipmentDefinition.Identifier,
                    IsDefect = model.IsDefect,
                    IsUsed = model.IsUsed,
                    SerialNumber = model.SerialNumber,
                    TemsId = model.TEMSID,
                    Type = model.EquipmentDefinition.EquipmentType.Name,
                    Personnnel = new Option
                    {
                        Value = (model.EquipmentAllocations.Count(q => q.DateReturned == null && q.PersonnelID != null) > 0)
                                ? model.EquipmentAllocations.FirstOrDefault().PersonnelID
                                : null,
                        Label = (model.EquipmentAllocations.Count(q => q.DateReturned == null && q.PersonnelID != null) > 0)
                                ? model.EquipmentAllocations.FirstOrDefault().Personnel.Name
                                : "TEMS",
                    },
                    Room = new Option
                    {
                        Value = (model.EquipmentAllocations.Count(q => q.DateReturned == null && q.RoomID != null) > 0)
                                ? model.EquipmentAllocations.FirstOrDefault().RoomID
                                : null,
                        Label = (model.EquipmentAllocations.Count(q => q.DateReturned == null && q.RoomID != null) > 0)
                                ? model.EquipmentAllocations.FirstOrDefault().Room.Identifier
                                : "Deposit",
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

                    SpecificTypeProperties = _mapper.Map<List<PropertyViewModel>>
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

        // -------------------------< Extract then to a separate file >--------------------------------
        private async Task<ViewEquipmentSimplifiedViewModel> EquipmentToEquipmentSimplifiedMapping(Equipment eq)
        {
            ViewEquipmentSimplifiedViewModel viewEquipmentSimplified = new ViewEquipmentSimplifiedViewModel
            {
                Id = eq.Id,
                IsDefect = eq.IsDefect,
                IsUsed = eq.IsUsed,
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
    }
}
