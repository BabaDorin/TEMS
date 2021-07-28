using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        [DefaultExceptionHandler("An error occured when creating the personnel")]
        public async Task<IActionResult> Create([FromBody] AddPersonnelViewModel viewModel)
        {
            var result = await _personnelManager.Create(viewModel);
            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPut("personnel/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while updating personnel data")]
        public async Task<IActionResult> Update([FromBody] AddPersonnelViewModel viewModel)
        {
            var result = await _personnelManager.Update(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("personnel/Archieve/{personnelId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while changing the archivation status")]
        public async Task<IActionResult> Archieve(string personnelId, bool archivationStatus = true)
        {
            var archievingResult = await (new ArchieveHelper(_unitOfWork, User))
                   .SetPersonnelArchivationStatus(personnelId, archivationStatus);
            if (archievingResult != null)
                return ReturnResponse(archievingResult, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("personnel/Remove/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the personnel")]
        public async Task<IActionResult> Remove(string personnelId)
        {
            string result = await _personnelManager.Remove(personnelId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("personnel/GetPositions")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching positions")]
        public async Task<IActionResult> GetPositions()
        {
            var positions = await _personnelManager.GetPositionOptions();
            return Ok(positions);
        }

        [HttpGet("personnel/GetSimplified/{pageNumber}/{recordsPerPage}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching personnel records")]
        public async Task<IActionResult> GetSimplified(int pageNumber, int recordsPerPage)
        {
            // Invalid page number or records per page provided
            if (pageNumber < 0 || recordsPerPage < 0 || pageNumber > 1000 || recordsPerPage > 1000)
                return ReturnResponse("Invalid parameters, please provide real number representing page number" +
                    "and how many records are displayed per page", ResponseStatus.Neutral);

            var simplifiedPersonnel = await _personnelManager.GetSimplified();
            return Ok(simplifiedPersonnel);
        }

        [HttpGet("personnel/GetAllAutocompleteOptions/{filter?}")]
        [DefaultExceptionHandler("An error occured when fetching autocomplete options")]
        public async Task<IActionResult> GetAllAutocompleteOptions(string filter)
        {
            var options = await _personnelManager.GetAutocompleteOptions(filter);
            return Ok(options);
        }

        [HttpGet("personnel/GetPersonnelToUpdate/{personnelId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while getting personnel's information")]
        public async Task<IActionResult> GetPersonnelToUpdate(string personnelId)
        {
            var personnel = await _personnelManager.GetById(personnelId);
            var viewModel = AddPersonnelViewModel.FromModel(personnel);
            return Ok(viewModel);
        }

        [HttpGet("personnel/GetById/{id}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching personnel information")]
        public async Task<IActionResult> GetById(string id)
        {
            var personnel = await _personnelManager.GetFullById(id);
            if (personnel == null)
                return ReturnResponse("Invalid personnel id", ResponseStatus.Neutral);

            var viewModel = ViewPersonnelViewModel.FromModel(personnel);
            return Ok(viewModel);
        }
    }
}
