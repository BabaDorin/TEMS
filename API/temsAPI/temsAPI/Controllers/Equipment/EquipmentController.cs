using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Equipment;

namespace temsAPI.Controllers.Equipment
{
    public class EquipmentController : TEMSController
    {

        public EquipmentController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
           : base(mapper, unitOfWork, userManager)
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Create([FromBody] AddEquipmentViewModel viewModel)
        {
            // one or both of TEMSID and SerialNumber properties should be given values
            if ((viewModel.Temsid == null ||
                String.IsNullOrEmpty(viewModel.Temsid = viewModel.Temsid.Trim())) &&
                (viewModel.SerialNumber == null ||
                String.IsNullOrEmpty(viewModel.SerialNumber = viewModel.SerialNumber.Trim())))
                return ReturnResponse("Please, provide information for TemsID and / or SerialNumber", Status.Fail);

            // Equipment is already created
            if (!String.IsNullOrEmpty(viewModel.Temsid) &&
                await _unitOfWork.Equipments.isExists(q => q.TEMSID == viewModel.Temsid) ||
                !String.IsNullOrEmpty(viewModel.SerialNumber) &&
                await _unitOfWork.Equipments.isExists(q => q.SerialNumber == viewModel.SerialNumber))
                return ReturnResponse("This equipment already exists.", Status.Fail);



            // Value for purchase date wasn't provided
            if (viewModel.PurchaseDate == new DateTime())
                viewModel.PurchaseDate = DateTime.Now;

            // Invalid price data
            if (viewModel.Price < 0 ||
                (new List<string> { "lei", "eur", "usd" }).IndexOf(viewModel.Currency) == -1)
                return ReturnResponse("Invalid price data provided.", Status.Fail);

            // Invalid definition provided
            if (!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == viewModel.EquipmentDefinitionID))
                return ReturnResponse("An equipment definition having the specified id has not been found.", Status.Fail);

            // If we got so far, it might be valid
            Data.Entities.EquipmentEntities.Equipment model =
                _mapper.Map<Data.Entities.EquipmentEntities.Equipment>(viewModel);

            model.Id = Guid.NewGuid().ToString();

            await _unitOfWork.Equipments.Create(model);
            await _unitOfWork.Save();

            if (!await _unitOfWork.Equipments.isExists(q => q.Id == model.Id))
                return ReturnResponse("Fail", Status.Fail);

            return ReturnResponse("Success", Status.Success);
        }

        [HttpGet("equipment/getsimplified/{pageNumber}/{equipmentsPerPage}/{onlyParents}")]
        public async Task<JsonResult> GetSimplified(int pageNumber, int equipmentsPerPage, bool onlyParents)
        {
            try
            {
                // Invalid parameters
                if (pageNumber < 0 || equipmentsPerPage < 1)
                    return ReturnResponse("Invalid parameters", Status.Fail);


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

                    var lastRoomAllocation = (await _unitOfWork.RoomEquipmentAllocations
                        .Find<RoomEquipmentAllocation>(q => q.EquipmentID == eq.Id && q.DateReturned == null,
                        include: q => q.Include(q => q.Room))).FirstOrDefault();

                    // If lastRoomAllocation is null, we check if it has been allocated to a personnel.
                    // If the equipment has not been allocated to any personnel or rooms, it belongs to 'Deposit'.
                    if (lastRoomAllocation == null)
                    {
                        var lastPersonnelAllocation = (await _unitOfWork.PersonnelEquipmentAllocations
                        .Find<PersonnelEquipmentAllocation>(q => q.EquipmentID == eq.Id && q.DateReturned == null,
                        include: q => q.Include(q => q.Personnel))).FirstOrDefault();

                        if (lastPersonnelAllocation == null)
                            viewEquipmentSimplified.Assignee = "Deposit";
                    }
                    else
                        viewEquipmentSimplified.Assignee = "Room: " + lastRoomAllocation.Room.Identifier;

                    viewEquipmentSimplified.TemsIdOrSerialNumber =
                        String.IsNullOrEmpty(viewEquipmentSimplified.TemsId)
                        ? viewEquipmentSimplified.SerialNumber
                        : viewEquipmentSimplified.TemsId;

                    viewModel.Add(viewEquipmentSimplified);
                }

                return Json(viewModel);
            }
            catch (Exception)
            {
                return ReturnResponse("Unknown error occured when fetching equipments", Status.Fail);
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
                return ReturnResponse("An error occured when fetching autocomplete options", Status.Fail);
            }
        }
    }
}
