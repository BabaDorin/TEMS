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
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Personnel;

namespace temsAPI.Controllers.PersonnelControllers
{
    public class PersonnelController : TEMSController
    {
        private PersonnelManager _personnelManager;

        public PersonnelController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            PersonnelManager personnelManager) : base(mapper, unitOfWork, userManager)
        {
            _personnelManager = personnelManager;
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetPositions()
        {
            try
            {
                var positions = await _personnelManager.GetPositionOptions();
                return Json(positions);
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
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return ReturnResponse("Invalid user provided", ResponseStatus.Fail);

                var personnel = await _personnelManager.GetById(personnelId);
                if (personnel == null)
                    return ReturnResponse("Invalid personnel provided", ResponseStatus.Fail);

                await _personnelManager.ConnectiWithUser(personnel, user);
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
                var result = await _personnelManager.Create(viewModel);
                return ReturnResponse("Success", ResponseStatus.Success);
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
                var archievingResult = await (new ArchieveHelper(_unitOfWork, User))
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

                var simplifiedPersonnel = await _personnelManager.GetSimplified();
                return Json(simplifiedPersonnel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured when fetching personnel records", ResponseStatus.Fail);
            }
        }

        [HttpGet("personnel/getallautocompleteoptions/{filter?}")]
        public async Task<JsonResult> GetAllAutocompleteOptions(string filter)
        {
            try
            {
                var options = await _personnelManager.GetAutocompleteOptions(filter);
                return Json(options);
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
                var personnel = await _personnelManager.GetById(personnelId);
                var viewModel = AddPersonnelViewModel.FromModel(personnel);
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
                var result = await _personnelManager.Update(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

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
                var personnel = await _personnelManager.GetFullById(id);
                if (personnel == null)
                    return ReturnResponse("Invalid personnel id", ResponseStatus.Fail);

                var viewModel = ViewPersonnelViewModel.FromModel(personnel);
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching personnel information", ResponseStatus.Fail);
                throw;
            }
        }
    }
}
