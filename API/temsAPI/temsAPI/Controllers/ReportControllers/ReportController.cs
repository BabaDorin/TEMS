using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Report;

namespace temsAPI.Controllers.ReportControllers
{
    public class ReportController : TEMSController
    {
        public ReportController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager) : base(mapper, unitOfWork, userManager)
        {
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> AddTemplate([FromBody] AddReportTemplateViewModel viewModel)
        {
            try
            {
                string validationMessage = await ValidateTemplate(viewModel);
                if (validationMessage != null)
                    return ReturnResponse(validationMessage, ResponseStatus.Fail);

                List<string> typeIds = viewModel.Types?.Select(q => q.Value).ToList();
                List<string> definitionIds = viewModel.Definitions?.Select(q => q.Value).ToList();
                List<string> roomIds = viewModel.Rooms?.Select(q => q.Value).ToList();
                List<string> personnelIds = viewModel.Personnel?.Select(q => q.Value).ToList();
                List<string> specificProperties = viewModel.SpecificProperties?.SelectMany(q => q.Properties).ToList();
                List<string> propertyIds = viewModel.CommonProperties?
                    .Concat(specificProperties == null ? new List<string>() : specificProperties)
                    .ToList();
                List<string> signatoriesIds = viewModel.Signatories?.Select(q => q.Value).ToList();

                ReportTemplate model = new ReportTemplate
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Subject = viewModel.Subject,
                    EquipmentTypes = (typeIds != null)
                        ? (await _unitOfWork.EquipmentTypes
                        .FindAll<EquipmentType>(q => typeIds.Contains(q.Id)))
                        .ToList()
                        : new List<EquipmentType>(),
                    EquipmentDefinitions = (definitionIds != null)
                        ? (await _unitOfWork.EquipmentDefinitions
                        .FindAll<EquipmentDefinition>(q => definitionIds.Contains(q.Id)))
                        .ToList()
                        : new List<EquipmentDefinition>(),
                    Rooms = (roomIds != null)
                        ? (await _unitOfWork.Rooms
                        .FindAll<Room>(q => roomIds.Contains(q.Id)))
                        .ToList()
                        : new List<Room>(),
                    Personnel = (personnelIds != null)
                        ? (await _unitOfWork.Personnel
                        .FindAll<Personnel>(q => personnelIds.Contains(q.Id)))
                        .ToList()
                        : new List<Personnel>(),
                    SepparateBy = viewModel.SepparateBy,
                    Properties = (propertyIds != null)
                        ? (await _unitOfWork.Properties
                        .FindAll<Property>(q => propertyIds.Contains(q.Id)))
                        .ToList()
                        : new List<Property>(),
                    Header = viewModel.Header,
                    Footer = viewModel.Footer,
                    Signatories = (signatoriesIds != null)
                        ? (await _unitOfWork.Personnel
                        .FindAll<Personnel>(q => signatoriesIds.Contains(q.Id)))
                        .ToList()
                        : new List<Personnel>()
                };

                await _unitOfWork.ReportTemplates.Create(model);
                await _unitOfWork.Save();

                if (!await _unitOfWork.ReportTemplates.isExists(q => q.Id == model.Id))
                    return ReturnResponse("Fail", ResponseStatus.Fail);

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while saving the template", ResponseStatus.Fail);
            }
        }




        // -----------------------------------------------------------------------

        /// <summary>
        /// Validates an instance of AddReportViewModel. Return null if everything is ok, otherwise - 
        /// returns an error message.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task<string> ValidateTemplate(AddReportTemplateViewModel viewModel)
        {
            // Invalid id provided (When it's the udpate case)
            if (viewModel.Id != null && !await _unitOfWork.ReportTemplates
                .isExists(q => q.Id == viewModel.Id))
                return "Invalid id provided";

            // Invalid subject
            if (new List<string>() { "equipment", "rooms", "personnel", "allocations" }
                .IndexOf(viewModel.Subject) == -1)
                return "Invalid subject";

            // Invalid types provided
            if(viewModel.Types != null)
                foreach (var item in viewModel.Types)
                    if (!await _unitOfWork.EquipmentTypes.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid type";

            // Invalid definitions provided
            if(viewModel.Definitions != null)
                foreach (var item in viewModel.Definitions)
                    if (!await _unitOfWork.EquipmentDefinitions.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid definition";

            // Invalid personnel provided
            if (viewModel.Personnel != null)
                foreach (var item in viewModel.Personnel)
                    if (!await _unitOfWork.Personnel.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid personnel";

            // Invalid rooms provided
            if (viewModel.Rooms != null)
                foreach (var item in viewModel.Rooms)
                    if (!await _unitOfWork.Rooms.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid room";

            // Invalid signatories provided
            if (viewModel.Signatories != null)
                foreach (var item in viewModel.Signatories)
                    if (!await _unitOfWork.Personnel.isExists(q => q.Id == item.Value))
                        return $"{item.Label} is not a valid type";

            // Invalid SepparateBy
            if (new List<string>() { "none", "room", "personnel", "type", "definition" }
                .IndexOf(viewModel.SepparateBy) == -1)
                return "Invalid SepparateBy";

            // Invalid properties
            List<string> commonTemsProps = new List<string>() { "temsid", "serialNumber", "price", "currency", "description", "identifier" };
            if(viewModel.CommonProperties != null)
                foreach (var item in viewModel.CommonProperties)
                    if (!commonTemsProps.Contains(item) && !await _unitOfWork.Properties.isExists(q => q.Name == item))
                        return $"{item} is not a valid property";

            var specificProperties = viewModel.SpecificProperties?.SelectMany(q => q.Properties).ToList();
            if(specificProperties != null)
                foreach (var item in specificProperties)
                    if (!await _unitOfWork.Properties.isExists(q => q.Name == item))
                        return $"{item} is not a valid property";

            return null;
        }
    }
}
