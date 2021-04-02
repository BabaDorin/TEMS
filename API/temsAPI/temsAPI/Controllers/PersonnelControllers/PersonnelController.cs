using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Personnel;

namespace temsAPI.Controllers.PersonnelControllers
{
    public class PersonnelController : TEMSController
    {
        public PersonnelController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetPositions()
        {
            try
            {
                List<Option> viewModel = (await _unitOfWork.PersonnelPositions.FindAll<Option>(
                    where: q => !q.IsArchieved,
                    select: q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }
                    )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching positions", ResponseStatus.Fail);
            }
        }

        [HttpGet("/personnel/connectwithuser/{personnelId}/{userId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> ConnectiWithUser(string personnelId, string userId)
        {
            try
            {
                var user = (await _unitOfWork.TEMSUsers
                    .Find<TEMSUser>(q => q.Id == userId))
                    .FirstOrDefault();

                if (user == null)
                    return ReturnResponse("Invalid user provided", ResponseStatus.Fail);

                var personnel = (await _unitOfWork.Personnel
                    .Find<Personnel>(q => q.Id == personnelId))
                    .FirstOrDefault();

                if (personnel == null)
                    return ReturnResponse("Invalid personnel provided", ResponseStatus.Fail);

                user.Personnel = personnel;
                await _unitOfWork.Save();

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while creating the personnel - user connection.", ResponseStatus.Fail);
            }
        }


        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Create([FromBody] AddPersonnelViewModel viewModel)
        {
            try
            {
                string validationResult = await ValidateAddPersonnelViewModel(viewModel);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                // It might be valid enough
                List<string> positions = viewModel.Positions.Select(q => q.Value).ToList();
                Personnel model = new Personnel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = viewModel.Name,
                    Email = viewModel.Email,
                    Positions = await _unitOfWork.PersonnelPositions.FindAll<PersonnelPosition>(
                        where: q => positions.Contains(q.Id))
                };

                await _unitOfWork.Personnel.Create(model);
                await _unitOfWork.Save();

                if (!await _unitOfWork.Personnel.isExists(q => q.Id == model.Id))
                    return ReturnResponse("Fail", ResponseStatus.Fail);
                else
                    return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when creating the personnel", ResponseStatus.Fail);
            }
        }

        [HttpGet("/personnel/archieve/{personnelId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Archieve(string personnelId, bool archivationStatus = true)
        {
            try
            {
                var archievingResult = await (new ArchieveHelper(_userManager, _unitOfWork))
                    .SetPersonnelArchivationStatus(personnelId, archivationStatus);
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


        [HttpGet("/personnel/getsimplified/{pageNumber}/{recordsPerPage}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetSimplified(int pageNumber, int recordsPerPage)
        {
            try
            {
                // Invalid page number or records per page provided
                if (pageNumber < 0 || recordsPerPage < 0 || pageNumber > 1000 || recordsPerPage > 1000)
                    return ReturnResponse("Invalid parameters, please provide real number representing page number" +
                        "and how many records are displayed per page", ResponseStatus.Fail);

                List<ViewPersonnelSimplifiedViewModel> viewModel = (await _unitOfWork.Personnel
                    .FindAll<ViewPersonnelSimplifiedViewModel>(
                        where: q => !q.IsArchieved,
                        include: q => q.Include(q => q.EquipmentAllocations)
                                       .Include(q => q.Positions)
                                       .Include(q => q.Tickets),
                        select: q => new ViewPersonnelSimplifiedViewModel
                        {
                            Id = q.Id,
                            Name = q.Name,
                            ActiveTickets = q.Tickets.Count(q => q.DateClosed == null),
                            AllocatedEquipments = q.EquipmentAllocations.Count(q => q.DateReturned == null),
                            Positions = (q.Positions != null)
                                ? string.Join(", ", q.Positions.Select(q => q.Name))
                                : "",
                            IsArchieved = q.IsArchieved
                        })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching personnel records", ResponseStatus.Fail);
            }
        }

        [HttpGet("personnel/getallautocompleteoptions/{filter?}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(string? filter)
        {
            try
            {
                Expression<Func<Personnel, bool>> expression = (filter == null)
                    ? q => !q.IsArchieved
                    : q => !q.IsArchieved && q.Name.Contains(filter);

                List<Option> viewModel = (await _unitOfWork.Personnel
                    .FindAll<Option>(
                        where: expression,
                        include: q => q.Include(q => q.Positions),
                        take: 5,
                        select: q => new Option
                        {
                            Value = q.Id,
                            Label = q.Name,
                            Additional = string.Join(", ", q.Positions.Select(q => q.Name))
                        })).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpGet("personnel/getpersonneltoupdate/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetPersonnelToUpdate(string personnelId)
        {
            try
            {
                var viewModel = (await _unitOfWork.Personnel
                    .Find<AddPersonnelViewModel>(
                        where: q => q.Id == personnelId,
                        include: q => q.Include(q => q.Positions),
                        select: q => new AddPersonnelViewModel
                        {
                            Id = q.Id,
                            Email = q.Email,
                            Name = q.Name,
                            PhoneNumber = q.PhoneNumber,
                            Positions = q.Positions.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.Name
                            }).ToList()
                        }
                    )).FirstOrDefault();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while getting personnel's information", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> Update([FromBody] AddPersonnelViewModel viewModel)
        {
            try
        {
                string validationResult = await ValidateAddPersonnelViewModel(viewModel);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var personnel = (await _unitOfWork.Personnel
                     .Find<Personnel>(
                     where: q => q.Id == viewModel.Id,
                     include: q => q.Include(q => q.Positions)
                    )).FirstOrDefault();

                personnel.Name = viewModel.Name;
                personnel.Email = viewModel.Email;
                personnel.PhoneNumber = viewModel.PhoneNumber;

                personnel.Positions.Clear();
                List<string> positionIds = viewModel.Positions.Select(q => q.Value).ToList();
                personnel.Positions = (await _unitOfWork.PersonnelPositions
                    .FindAll<PersonnelPosition>
                    (
                        where: q => positionIds.Contains(q.Id)
                    )).ToList();

                await _unitOfWork.Save();
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while saving personnel data", ResponseStatus.Fail);
                throw;
            }
        }

        [HttpGet("personnel/getbyid/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetById(string id)
        {
            try
            {
                // Invalid id
                if (String.IsNullOrEmpty(id = id.Trim()))
                    return ReturnResponse("Please, provide a valid Id", ResponseStatus.Fail);

                // No match
                if (!await _unitOfWork.Personnel.isExists(q => q.Id == id))
                    return ReturnResponse("No personnel found", ResponseStatus.Fail);

                ViewPersonnelViewModel viewModel = (await _unitOfWork.Personnel
                    .Find<ViewPersonnelViewModel>(
                        where: q => q.Id == id,
                        include: q => q.Include(q => q.Logs)
                                       .Include(q => q.Tickets)
                                       .Include(q => q.Positions)
                                       .Include(q => q.EquipmentAllocations)
                                       .Include(q => q.PersonnelRoomSupervisories).ThenInclude(q => q.Room),
                        select: q => new ViewPersonnelViewModel
                        {
                            Id = q.Id,
                            Name = q.Name,
                            Email = q.Email,
                            PhoneNumber = q.PhoneNumber,
                            IsArchieved = q.IsArchieved,
                            ActiveTickets = q.Tickets.Count(q => q.DateClosed == null),
                            AllocatedEquipments = q.EquipmentAllocations.Count(q => q.DateReturned == null),
                            Positions = q.Positions.Select(q => new Option
                            {
                                Value = q.Id,
                                Label = q.Name
                            }).ToList(),
                            RoomSupervisories = q.PersonnelRoomSupervisories.Select(q => new Option
                            {
                                Value = q.RoomID,
                                Label = q.Room.Identifier
                            }).ToList(),
                        }
                    )).FirstOrDefault();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching personnel information", ResponseStatus.Fail);
                throw;
            }
        }

        // ----------------------------------------------------
        
        /// <summary>
        /// Validates an instance of AddPersonnelViewModel. Returns null if everything is ok,
        /// otherwise returns the error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<string> ValidateAddPersonnelViewModel(AddPersonnelViewModel viewModel)
        {
            // It's the udpate case and the provided is is invalid
            Personnel personnelToUpdate = null;
            if(viewModel.Id != null)
            {
                personnelToUpdate = (await _unitOfWork.Personnel
                    .Find<Personnel>(q => q.Id == viewModel.Id))
                    .FirstOrDefault();

                if (personnelToUpdate == null)
                    return "Invalid personnel Id";
            }

            // Invalid Name
            if (String.IsNullOrEmpty(viewModel.Name = viewModel.Name.Trim()))
                return "Invalid personnel name provided";

            // Checking for invalid personnel positions
            foreach (Option position in viewModel.Positions)
                if (!await _unitOfWork.PersonnelPositions
                    .isExists(q => q.Id == position.Value && !q.IsArchieved))
                    return "One or more positions are invalid.";

            return null;
        }
    }
}
