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
using temsAPI.ViewModels.EquipmentDefinition;
using static temsAPI.Data.Managers.EquipmentDefinitionManager;

namespace temsAPI.Controllers.EquipmentControllers
{
    public class EquipmentDefinitionController : TEMSController
    {
        private EquipmentDefinitionManager _equipmentDefinitionManager;
        public EquipmentDefinitionController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            EquipmentDefinitionManager equipmentDefinitionManager,
            ILogger<TEMSController> logger)
           : base(mapper, unitOfWork, userManager, logger)
        {
            _equipmentDefinitionManager = equipmentDefinitionManager;
        }

        [HttpPost("equipmentdefinition/Add")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while adding the definition. Property values might be too large (maximum length for property value: 1500 characters)")]
        public async Task<IActionResult> Add([FromBody] AddEquipmentDefinitionViewModel viewModel)
        {
            string result = await _equipmentDefinitionManager.Create(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpPut("equipmentdefinition/Update")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while updating the definition")]
        public async Task<IActionResult> Update([FromBody] AddEquipmentDefinitionViewModel viewModel)
        {
            var result = await _equipmentDefinitionManager.Update(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success!", ResponseStatus.Success);
        }

        [HttpGet("equipmentdefinition/Archieve/{definitionId}/{archivationStatus?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while changing the archivation status.")]
        public async Task<IActionResult> Archieve(string definitionId, bool archivationStatus = true)
        {
            var archievingResult = await (new ArchieveHelper(_unitOfWork, User))
                    .SetDefinitionArchivationStatus(definitionId, archivationStatus);

            if (archievingResult != null)
                return ReturnResponse(archievingResult, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("equipmentdefinition/Remove/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the definition")]
        public async Task<IActionResult> Remove(string definitionId)
        {
            string result = await _equipmentDefinitionManager.Remove(definitionId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Fail);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("equipmentdefinition/GetDefinitionsOfType/{typeId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching definitions of specified type")]
        public async Task<IActionResult> GetDefinitionsOfType(string typeId)
        {
            var options = await _equipmentDefinitionManager.GetOfType(typeId);
            return Ok(options);
        }

        [HttpPost("equipmentdefinition/GetDefinitionsOfTypes")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching definitions of specified types")]
        public async Task<IActionResult> GetDefinitionsOfTypes([FromBody] DefinitionsOfTypesModel filter)
        {
            var options = await _equipmentDefinitionManager.GetOfTypes(filter);
            return Ok(options);
        }

        [HttpGet("equipmentdefinition/GetSimplified")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching definitions")]
        public async Task<IActionResult> GetSimplified()
        {
            var definitions = await _equipmentDefinitionManager.GetSimplified();
            return Ok(definitions);
        }

        [HttpGet("equipmentdefinition/GetSimplifiedById/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the definition")]
        public async Task<IActionResult> GetSimplifiedById(string definitionId)
        {
            var definition = await _equipmentDefinitionManager.GetSimplifiedById(definitionId);
            return Ok(definition);
        }

        [HttpGet("equipmentdefinition/GetDefinitionToUpdate/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching the definition")]
        public async Task<IActionResult> GetDefinitionToUpdate(string definitionId)
        {
            var definition = await _equipmentDefinitionManager.GetFullById(definitionId);
            if (definition == null)
                return ReturnResponse("Invalid definition id provided", ResponseStatus.Fail);

            var viewModel = AddEquipmentDefinitionViewModel.FromModel(definition);
            return Ok(viewModel);
        }

        [HttpGet("equipmentdefinition/GetFullDefinition/{definitionId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured when fetching defintion's data")]
        public async Task<IActionResult> GetFullDefinition(string definitionId)
        {
            var definition = await _equipmentDefinitionManager.GetFullById(definitionId);
            if (definition == null)
                return ReturnResponse("Invalid definition Id", ResponseStatus.Fail);

            var viewModel = EquipmentDefinitionViewModel.FromModel(definition);
            return Ok(viewModel);
        }
    }
}
