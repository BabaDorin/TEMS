using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using temsAPI.Helpers;
using temsAPI.Services.Report;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Report;

namespace temsAPI.Controllers.ReportControllers
{
    public class ReportController : TEMSController
    {
        ReportingService _reportingService;

        public ReportController(
            IMapper mapper, 
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            ReportingService reportingService) : base(mapper, unitOfWork, userManager)
        {
            _reportingService = reportingService;
        }

        [HttpGet("report/gettemplatetoupdate/{templateId}")]
        public async Task<JsonResult> GetTemplateToUpdate(string templateId)
        {
            try
            {
                ReportTemplate model = (await _unitOfWork.ReportTemplates
                    .FindAll<ReportTemplate>(
                        where: q => q.Id == templateId,
                        include: q => q
                        .Include(q => q.EquipmentTypes)
                        .Include(q => q.EquipmentDefinitions)
                        .Include(q => q.Rooms)
                        .Include(q => q.Personnel)
                        .Include(q => q.Properties)
                        .Include(q => q.Signatories)
                    )).FirstOrDefault();

                if (model == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                var viewModel = new AddReportTemplateViewModel
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Subject = model.Subject,
                    Types = model.EquipmentTypes.Select(q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }).ToList(),
                    Definitions = model.EquipmentDefinitions.Select(q => new Option
                    {
                        Value = q.Id,
                        Label = q.Identifier
                    }).ToList(),
                    Rooms = model.Rooms.Select(q => new Option
                    {
                        Value = q.Id,
                        Label = q.Identifier
                    }).ToList(),
                    Personnel = model.Personnel.Select(q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }).ToList(),
                    Properties = model.Properties.Select(q => q.Name).ToList(),
                    SepparateBy = model.SepparateBy,
                    Header = model.Header,
                    Footer = model.Footer,
                    Signatories = model.Signatories.Select(q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }).ToList(),
                };

                if (model.UniversalProperties != null)
                    viewModel.Properties = viewModel.Properties
                        .Concat(model.UniversalProperties.Split(' '))
                        .ToList();

                return Json(viewModel); // update it    
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching the template", ResponseStatus.Fail);
            }
        }

        [HttpGet("report/archievetemplate/{templateId}/{flag?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> ArchieveTemplate(string templateId, bool flag = true)
        {
            try
            {
                string archivationResult = await (new ArchieveHelper(_userManager, _unitOfWork))
                    .SetReportTemplateArchivationStatus(templateId, flag);
                if (archivationResult != null)
                    return ReturnResponse(archivationResult, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while removing the template", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> AddTemplate([FromBody] AddReportTemplateViewModel viewModel)
        {
            try
            {
                string validationMessage = await viewModel.Validate(_unitOfWork);
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
                List<string> universalProperties = viewModel.CommonProperties.Where(q => ReportHelper.UniversalProperties.Contains(q)).ToList();
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
                        .FindAll<Property>(q => propertyIds.Contains(q.Name)))
                        .ToList()
                        : new List<Property>(),
                    Header = viewModel.Header,
                    Footer = viewModel.Footer,
                    Signatories = (signatoriesIds != null)
                        ? (await _unitOfWork.Personnel
                        .FindAll<Personnel>(q => signatoriesIds.Contains(q.Id)))
                        .ToList()
                        : new List<Personnel>(),
                    CreatedBy = (await _unitOfWork.TEMSUsers
                        .Find<TEMSUser>(
                            where: q => q.Id == IdentityHelper.GetUserId(User)
                        )).FirstOrDefault(),
                    DateCreated = DateTime.Now,
                    UniversalProperties = (universalProperties.Count > 0)
                        ? String.Join(" ", universalProperties)
                        : null
                };

                var id = IdentityHelper.GetUserId(User);

                if (User.Identity.Name == "tems@admin")
                {
                    model.CreatedBy = null;
                    model.CreatedById = null;
                }

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

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> UpdateTemplate([FromBody] AddReportTemplateViewModel viewModel)
        {
            try
            {
                string validationMessage = await viewModel.Validate(_unitOfWork);
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
                List<string> universalProperties = viewModel.CommonProperties.Where(q => ReportHelper.UniversalProperties.Contains(q)).ToList();
                List<string> signatoriesIds = viewModel.Signatories?.Select(q => q.Value).ToList();

                var model = (await _unitOfWork.ReportTemplates
                    .Find<ReportTemplate>(
                        where: q => q.Id == viewModel.Id,
                        include: q => q
                        .Include(q => q.EquipmentTypes)
                        .Include(q => q.EquipmentDefinitions)
                        .Include(q => q.Rooms)
                        .Include(q => q.Personnel)
                        .Include(q => q.Properties)
                        .Include(q => q.Signatories)
                    )).FirstOrDefault();

                model.Name = viewModel.Name;
                model.Description = viewModel.Description;
                model.Subject = viewModel.Subject;
                model.EquipmentTypes = (typeIds != null)
                        ? (await _unitOfWork.EquipmentTypes
                        .FindAll<EquipmentType>(q => typeIds.Contains(q.Id)))
                        .ToList()
                        : new List<EquipmentType>();
                model.EquipmentDefinitions = (definitionIds != null)
                        ? (await _unitOfWork.EquipmentDefinitions
                        .FindAll<EquipmentDefinition>(q => definitionIds.Contains(q.Id)))
                        .ToList()
                        : new List<EquipmentDefinition>();
                model.Rooms = (roomIds != null)
                        ? (await _unitOfWork.Rooms
                        .FindAll<Room>(q => roomIds.Contains(q.Id)))
                        .ToList()
                        : new List<Room>();
                model.Personnel = (personnelIds != null)
                        ? (await _unitOfWork.Personnel
                        .FindAll<Personnel>(q => personnelIds.Contains(q.Id)))
                        .ToList()
                        : new List<Personnel>();
                model.SepparateBy = viewModel.SepparateBy;
                model.Properties = (propertyIds != null)
                        ? (await _unitOfWork.Properties
                        .FindAll<Property>(q => propertyIds.Contains(q.Name)))
                        .ToList()
                        : new List<Property>();
                model.Header = viewModel.Header;
                model.Footer = viewModel.Footer;
                model.Signatories = (signatoriesIds != null)
                        ? (await _unitOfWork.Personnel
                        .FindAll<Personnel>(q => signatoriesIds.Contains(q.Id)))
                        .ToList()
                        : new List<Personnel>();
                model.CreatedBy = (await _unitOfWork.TEMSUsers
                        .Find<TEMSUser>(
                            where: q => q.UserName == User.Identity.Name
                        )).FirstOrDefault();
                model.UniversalProperties = (universalProperties.Count > 0)
                        ? String.Join(" ", universalProperties)
                        : null;

                await _unitOfWork.Save();
                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while saving the template", ResponseStatus.Fail);
            }
        }

        [HttpGet("report/generatereport/{templateId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GenerateReport(string templateId)
        {
            var reportTemplate = (await _unitOfWork.ReportTemplates
                .Find<ReportTemplate>(
                    where: q => q.Id == templateId,
                    include: q => q
                    .Include(q => q.CreatedBy)
                    .Include(q => q.EquipmentDefinitions)
                    .Include(q => q.EquipmentTypes)
                    .Include(q => q.Personnel)
                    .Include(q => q.Properties).ThenInclude(q => q.DataType)
                    .Include(q => q.Rooms)
                    .Include(q => q.Signatories)
                    )).FirstOrDefault();

            if (reportTemplate == null)
                return ReturnResponse("Invalid template ID provided", ResponseStatus.Fail);

            var excelReport = await _reportingService.GenerateReport(reportTemplate);

            return ReturnResponse("Will be implemented soon", ResponseStatus.Success);
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetTemplates()
        {
            try
            {
                var viewModel = (await _unitOfWork.ReportTemplates
                    .FindAll<ViewReportTemplateSimplifiedViewModel>(
                        where: q => !q.IsArchieved,
                        include: q => q
                        .Include(q => q.CreatedBy),
                        select: q => new ViewReportTemplateSimplifiedViewModel
                        {
                            Id = q.Id,
                            CreatedBy = new Option
                            {
                                Value = q.CreatedById,
                                Label = q.CreatedBy.UserName,
                            },
                            DateCreated = q.DateCreated,
                            Description = q.Description,
                            IsDefault = q.CreatedById == null,
                            Name = q.Name
                        }
                    )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching report templates", ResponseStatus.Fail);
            }
        }
    }
}
