using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
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
            PersonnelManager personnelManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _personnelManager = personnelManager;
        }

        [HttpPost("personnel/Create")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Create([FromBody] AddPersonnelViewModel viewModel)
        {
            try
            {
                var result = await _personnelManager.Create(viewModel);
                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when creating the personnel", ResponseStatus.Fail);
            }
        }

        [HttpPut("personnel/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Update([FromBody] AddPersonnelViewModel viewModel)
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
                LogException(ex);
                return ReturnResponse("An error occured while saving personnel data", ResponseStatus.Fail);
                throw;
            }
        }

        [HttpGet("personnel/Archieve/{personnelId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> Archieve(string personnelId, bool archivationStatus = true)
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
                LogException(ex);
                return ReturnResponse("An error occured while changing the archivation status.", ResponseStatus.Fail);
            }
        }

        [HttpDelete("personnel/Remove/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<IActionResult> Remove(string personnelId)
        {
            try
            {
                string result = await _personnelManager.Remove(personnelId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the personnel", ResponseStatus.Fail);
            }
        }

        [HttpGet("personnel/GetPositions")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetPositions()
        {
            try
            {
                var positions = await _personnelManager.GetPositionOptions();
                return Ok(positions);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching positions", ResponseStatus.Fail);
            }
        }

        [HttpGet("personnel/GetSimplified/{pageNumber}/{recordsPerPage}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetSimplified(int pageNumber, int recordsPerPage)
        {
            try
            {
                // Invalid page number or records per page provided
                if (pageNumber < 0 || recordsPerPage < 0 || pageNumber > 1000 || recordsPerPage > 1000)
                    return ReturnResponse("Invalid parameters, please provide real number representing page number" +
                        "and how many records are displayed per page", ResponseStatus.Fail);

                var simplifiedPersonnel = await _personnelManager.GetSimplified();
                return Ok(simplifiedPersonnel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching personnel records", ResponseStatus.Fail);
            }
        }

        [HttpGet("personnel/GetAllAutocompleteOptions/{filter?}")]
        public async Task<IActionResult> GetAllAutocompleteOptions(string filter)
        {
            try
            {
                var options = await _personnelManager.GetAutocompleteOptions(filter);
                return Ok(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured when fetching autocomplete options", ResponseStatus.Fail);
            }
        }

        [HttpGet("personnel/GetPersonnelToUpdate/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetPersonnelToUpdate(string personnelId)
        {
            try
            {
                var personnel = await _personnelManager.GetById(personnelId);
                var viewModel = AddPersonnelViewModel.FromModel(personnel);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while getting personnel's information", ResponseStatus.Fail);
            }
        }

        [HttpGet("personnel/GetById/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var personnel = await _personnelManager.GetFullById(id);
                if (personnel == null)
                    return ReturnResponse("Invalid personnel id", ResponseStatus.Fail);

                var viewModel = ViewPersonnelViewModel.FromModel(personnel);
                return Ok(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching personnel information", ResponseStatus.Fail);
                throw;
            }
        }
    }
}
