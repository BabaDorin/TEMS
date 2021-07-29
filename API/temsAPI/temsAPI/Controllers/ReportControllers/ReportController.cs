using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Data.Managers;
using temsAPI.Helpers;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services.Report;
using temsAPI.System_Files;
using temsAPI.System_Files.Exceptions;
using temsAPI.ViewModels.Report;

namespace temsAPI.Controllers.ReportControllers
{
    public class ReportController : TEMSController
    {
        readonly ReportingService _reportingService;
        readonly ReportManager _reportManager;
        readonly GeneratedReportFileHandler fileHandler = new();
        
        public ReportController(
            IUnitOfWork unitOfWork, 
            UserManager<TEMSUser> userManager,
            ReportingService reportingService,
            ReportManager reportManager,
            ILogger<TEMSController> logger) : base(unitOfWork, userManager, logger)
        {
            _reportingService = reportingService;
            _reportManager = reportManager;
        }

        [HttpPost("report/AddTemplate")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while saving the template")]
        public async Task<IActionResult> AddTemplate([FromBody] AddReportTemplateViewModel viewModel)
        {
            var result = await _reportManager.CreateTemplate(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse($"Success!", ResponseStatus.Success);
        }

        [HttpPut("report/UpdateTemplate")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while updating the template")]
        public async Task<IActionResult> UpdateTemplate([FromBody] AddReportTemplateViewModel viewModel)
        {
            var result = await _reportManager.UpdateTemplate(viewModel);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success!", ResponseStatus.Success);
        }

        [HttpGet("report/ArchieveTemplate/{templateId}/{flag?}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while removing the template")]
        public async Task<IActionResult> ArchieveTemplate(string templateId, bool flag = true)
        {
            string archivationResult = await (new ArchieveHelper(_unitOfWork, User))
                    .SetReportTemplateArchivationStatus(templateId, flag);
            if (archivationResult != null)
                return ReturnResponse(archivationResult, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpDelete("report/RemoveReport/{reportId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while removing the report")]
        public async Task<IActionResult> RemoveReport(string reportId)
        {
            var report = await _reportManager.GetReport(reportId);
            if (report == null)
                return ReturnResponse("Invalid id provided", ResponseStatus.Neutral);

            string result = await _reportManager.RemoveReport(report);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("report/GetTemplateToUpdate/{templateId}")]
        [DefaultExceptionHandler("An error occured while fetching the template")]
        public async Task<IActionResult> GetTemplateToUpdate(string templateId)
        {
            var template = await _reportManager.GetFullTemplate(templateId);
            if (template == null)
                return ReturnResponse("Invalid id provided", ResponseStatus.Neutral);

            var viewModel = AddReportTemplateViewModel.FromModel(template);
            return Ok(viewModel);
        }

        [HttpDelete("report/Remove/{templateId}")]
        [ClaimRequirement(TEMSClaims.CAN_MANAGE_SYSTEM_CONFIGURATION)]
        [DefaultExceptionHandler("An error occured while removing the template")]
        public async Task<IActionResult> Remove(string templateId)
        {
            string result = await _reportManager.Remove(templateId);
            if (result != null)
                return ReturnResponse(result, ResponseStatus.Neutral);

            return ReturnResponse("Success", ResponseStatus.Success);
        }

        [HttpGet("report/GenerateReport/{templateId}"), DisableRequestSizeLimit]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while generating the report")]
        public async Task<IActionResult> GenerateReport(string templateId)
        {
            var template = await _reportManager.GetFullTemplate(templateId);
            if (template == null)
                return ReturnResponse("Invalid id provided", ResponseStatus.Neutral);

            var report = await _reportManager.CreateReport(template);

            var memory = await _reportManager.GetReportMemoryStream(report.DBPath);
            if (memory == null)
                return NotFound();

            return File(memory, fileHandler.GetContentType(report.DBPath), "Report.xlsx");
        }

        [HttpGet("report/GetTemplates")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching report templates")]
        public async Task<IActionResult> GetTemplates()
        {
            var reportTemplates = await _reportManager.GetReportTemplates();
            return Ok(reportTemplates);
        }

        [HttpGet("report/GetLastGeneratedReports")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while fetching last generated reports")]
        public async Task<IActionResult> GetLastGeneratedReports()
        {
            var reports = await _reportManager.GetLastGeneratedReports();
            return Ok(reports);
        }

        [HttpPost("report/GenerateReportFromRawTemplate")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while generating the report")]
        public async Task<IActionResult> GenerateReportFromRawTemplate([FromBody] AddReportTemplateViewModel template)
        {
            string validationResult = await template.Validate(_unitOfWork);
            if (validationResult != null)
                return ReturnResponse(validationResult, ResponseStatus.Neutral);

            var reportTemplate = await template.ToModel(_unitOfWork, new TEMSUser());

            string filePath = fileHandler.GetTempDBPath();
            await _reportingService.GenerateReport(reportTemplate, filePath);

            var memory = await _reportManager.GetReportMemoryStream(filePath);
            return File(memory, fileHandler.GetContentType(filePath), "Report.xlsx");
        }

        [HttpGet("report/GetReport/{reportId}")]
        [ClaimRequirement(TEMSClaims.CAN_VIEW_ENTITIES, TEMSClaims.CAN_MANAGE_ENTITIES)]
        [DefaultExceptionHandler("An error occured while retrieving the report")]
        public async Task<IActionResult> GetReport(string reportId)
        {
            var report = await _reportManager.GetReport(reportId);
            if (report == null)
                return NotFound();

            var memory = await _reportManager.GetReportMemoryStream(report.DBPath);
            return File(memory, fileHandler.GetContentType(report.DBPath), "Report.xlsx");
        }
    }
}
