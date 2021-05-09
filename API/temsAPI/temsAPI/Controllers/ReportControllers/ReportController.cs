﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services;
using temsAPI.Services.Report;
using temsAPI.System_Files;
using temsAPI.ViewModels;
using temsAPI.ViewModels.Report;

namespace temsAPI.Controllers.ReportControllers
{
    public class ReportController : TEMSController
    {
        ReportingService _reportingService;
        AppSettings _appSettings;
        ReportManager _reportManager;
        GeneratedReportFileHandler fileHandler = new();
        
        public ReportController(
            IMapper mapper,
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            ReportingService reportingService,
            IOptions<AppSettings> appSettings,
            ReportManager reportManager) : base(mapper, unitOfWork, userManager)
        {
            _reportingService = reportingService;
            _appSettings = appSettings.Value;
            _reportManager = reportManager;
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
                    SeparateBy = model.SeparateBy,
                    Header = model.Header,
                    Footer = model.Footer,
                    Signatories = model.Signatories.Select(q => new Option
                    {
                        Value = q.Id,
                        Label = q.Name
                    }).ToList(),
                };

                if (model.CommonProperties != null)
                    viewModel.Properties = viewModel.Properties
                        .Concat(model.CommonProperties.Split(' '))
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
                string archivationResult = await (new ArchieveHelper(_unitOfWork, User))
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

                var user = (await _unitOfWork.TEMSUsers
                    .Find<TEMSUser>(q => q.Id == IdentityHelper.GetUserId(User)))
                    .FirstOrDefault();

                if (user == null)
                    return ReturnResponse("An error occured - Invalid user", ResponseStatus.Fail);

                var model = await viewModel.ToReportTemplate(_unitOfWork, user);

                await _unitOfWork.ReportTemplates.Create(model);
                await _unitOfWork.Save();

                return ReturnResponse($"Success!", ResponseStatus.Success);
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
                List<string> universalProperties = viewModel.CommonProperties.Where(q => ReportHelper.CommonProperties.Contains(q)).ToList();
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
                model.SeparateBy = viewModel.SeparateBy;
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
                model.CommonProperties = (universalProperties.Count > 0)
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

        [HttpGet("report/generatereport/{templateId}"), DisableRequestSizeLimit]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<IActionResult> GenerateReport(string templateId)
        {
            try
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

                Report report = new();
                report.Id = Guid.NewGuid().ToString();
                report.Template = reportTemplate.Name;
                report.GeneratedByID = IdentityHelper.GetUserId(User);
                report.DateGenerated = DateTime.Now;

                report.DBPath = fileHandler.GetDBPath();

                var excelReport = await _reportingService.GenerateReport(reportTemplate, report.DBPath);
                
                await _reportManager.CheckForReportsOverflow();
                
                await _unitOfWork.Reports.Create(report);
                await _unitOfWork.Save();

                var memory = await _reportManager.GetReportMemoryStream(report.DBPath);
                if (memory == null)
                    return NotFound();

                return File(memory, fileHandler.GetContentType(report.DBPath), "Report.xlsx");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while preparing the report", ResponseStatus.Fail);
            }
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

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<JsonResult> GetLastGeneratedReports()
        {
            try
            {
                var viewModel = (await _unitOfWork.Reports
                    .Find(
                        include: q => q.Include(q => q.GeneratedBy),
                        select: q => new ViewGeneratedReportViewModel
                        {
                            Id = q.Id,
                            DateGenerated = q.DateGenerated,
                            GeneratedBy = new Option
                            {
                                Value = q.GeneratedBy.Id,
                                Label = q.GeneratedBy.FullName ?? q.GeneratedBy.UserName
                            },
                            Template = q.Template
                        }
                    )).ToList();

                return Json(viewModel);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while fetching last generated reports", ResponseStatus.Fail);
            }
        }

        [HttpGet("report/removeReport/{reportId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> RemoveReport(string reportId)
        {
            try
            {
                var report = await _reportManager.GetReport(reportId);
                if (report == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                string result = await _reportManager.RemoveReport(report);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while removing the report", ResponseStatus.Fail);
            }
        }

        [HttpPost("report/generatereportfromrawtemplate")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<IActionResult> GenerateReportFromRawTemplate([FromBody] AddReportTemplateViewModel template)
        {
            try
            {
                string validationResult = await template.Validate(_unitOfWork);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var reportTemplate = await template.ToReportTemplate(_unitOfWork, new TEMSUser());
                
                string filePath = fileHandler.GetTempDBPath();
                var excelFile = await _reportingService.GenerateReport(reportTemplate, filePath);
                
                var memory = await _reportManager.GetReportMemoryStream(filePath);
                return File(memory, fileHandler.GetContentType(filePath), "Report.xlsx");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return NoContent();
            }
        }

        [HttpGet("report/getreport/{reportId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES)]
        public async Task<IActionResult> GetReport(string reportId)
        {
            try
            {
                var report = await _reportManager.GetReport(reportId);
                if (report == null)
                    return NotFound();

                var memory = await _reportManager.GetReportMemoryStream(report.DBPath);
                return File(memory, fileHandler.GetContentType(report.DBPath), "Report.xlsx");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return ReturnResponse("An error occured while retrieving the report", ResponseStatus.Fail);
            }
        }
    }
}
