using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services.Report;
using temsAPI.System_Files;
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
            ReportManager reportManager,
            ILogger<TEMSController> logger) : base(mapper, unitOfWork, userManager, logger)
        {
            _reportingService = reportingService;
            _appSettings = appSettings.Value;
            _reportManager = reportManager;
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> AddTemplate([FromBody] AddReportTemplateViewModel viewModel)
        {
            try
            {
                var result = await _reportManager.CreateTemplate(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse($"Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while saving the template", ResponseStatus.Fail);
            }
        }

        [HttpPost]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> UpdateTemplate([FromBody] AddReportTemplateViewModel viewModel)
        {
            try
            {
                var result = await _reportManager.UpdateTemplate(viewModel);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success!", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while saving the template", ResponseStatus.Fail);
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
                LogException(ex);
                return ReturnResponse("An error occured while removing the template", ResponseStatus.Fail);
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
                LogException(ex);
                return ReturnResponse("An error occured while removing the report", ResponseStatus.Fail);
            }
        }

        [HttpGet("report/gettemplatetoupdate/{templateId}")]
        public async Task<JsonResult> GetTemplateToUpdate(string templateId)
        {
            try
            {
                var template = await _reportManager.GetFullTemplate(templateId);
                if (template == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);

                var viewModel = AddReportTemplateViewModel.FromModel(template);
                return Json(viewModel);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching the template", ResponseStatus.Fail);
            }
        }

        [HttpGet("report/remove/{templateId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        public async Task<JsonResult> ArchieveTemplate(string templateId)
        {
            try
            {
                string result = await _reportManager.Remove(templateId);
                if (result != null)
                    return ReturnResponse(result, ResponseStatus.Fail);

                return ReturnResponse("Success", ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while removing the template", ResponseStatus.Fail);
            }
        }

        [HttpGet("report/generatereport/{templateId}"), DisableRequestSizeLimit]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GenerateReport(string templateId)
        {
            try
            {
                var template = await _reportManager.GetFullTemplate(templateId);
                if (template == null)
                    return ReturnResponse("Invalid id provided", ResponseStatus.Fail);
                
                var report = await _reportManager.CreateReport(template);
                
                var memory = await _reportManager.GetReportMemoryStream(report.DBPath);
                if (memory == null)
                    return NotFound();

                return File(memory, fileHandler.GetContentType(report.DBPath), "Report.xlsx");
            }
            catch (Exception ex)
            {
                LogException(ex);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetTemplates()
        {
            try
            {
                var reportTemplates = await _reportManager.GetReportTemplates();
                return Json(reportTemplates);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching report templates", ResponseStatus.Fail);
            }
        }

        [HttpGet]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<JsonResult> GetLastGeneratedReports()
        {
            try
            {
                var reports = await _reportManager.GetLastGeneratedReports();
                return Json(reports);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return ReturnResponse("An error occured while fetching last generated reports", ResponseStatus.Fail);
            }
        }

        [HttpPost("report/generatereportfromrawtemplate")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        public async Task<IActionResult> GenerateReportFromRawTemplate([FromBody] AddReportTemplateViewModel template)
        {
            try
            {
                string validationResult = await template.Validate(_unitOfWork);
                if (validationResult != null)
                    return ReturnResponse(validationResult, ResponseStatus.Fail);

                var reportTemplate = await template.ToModel(_unitOfWork, new TEMSUser());
                
                string filePath = fileHandler.GetTempDBPath();
                var excelFile = await _reportingService.GenerateReport(reportTemplate, filePath);
                
                var memory = await _reportManager.GetReportMemoryStream(filePath);
                return File(memory, fileHandler.GetContentType(filePath), "Report.xlsx");
            }
            catch (Exception ex)
            {
                LogException(ex);
                return NoContent();
            }
        }

        [HttpGet("report/getreport/{reportId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
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
                LogException(ex);
                return ReturnResponse("An error occured while retrieving the report", ResponseStatus.Fail);
            }
        }
    }
}
