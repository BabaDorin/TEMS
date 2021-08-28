using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.Helpers.StaticFileHelpers;
using temsAPI.Services;
using temsAPI.Services.Report;
using temsAPI.System_Files;
using temsAPI.ViewModels.Report;

namespace temsAPI.Data.Managers
{
    public class ReportManager
    {
        private IUnitOfWork _unitOfWork;
        GeneratedReportFileHandler fileHandler = new();
        AppSettings _appSettings;
        IdentityService _identityService;
        ReportingService _reportingService;

        public ReportManager(
            IUnitOfWork unitOfWork,
            IOptions<AppSettings> appSettings,
            IdentityService identityService,
            ReportingService reportingService)
        {
            _unitOfWork = unitOfWork;
            _appSettings = appSettings.Value;
            _identityService = identityService;
            _reportingService = reportingService;
        }

        public async Task<string> CreateTemplate(AddReportTemplateViewModel viewModel)
        {
            string validationMessage = await viewModel.Validate(_unitOfWork);
            if (validationMessage != null)
                return validationMessage;

            var user = (await _unitOfWork.TEMSUsers
                .Find<TEMSUser>(q => q.Id == _identityService.GetUserId()))
                .FirstOrDefault();
            if (user == null)
                return "An error occured - Invalid user";

            var model = await viewModel.ToModel(_unitOfWork, user);

            await _unitOfWork.ReportTemplates.Create(model);
            await _unitOfWork.Save();

            return null;
        }

        public async Task<string> UpdateTemplate(AddReportTemplateViewModel viewModel)
        {
            string validationMessage = await viewModel.Validate(_unitOfWork);
            if (validationMessage != null)
                return validationMessage;

            List<string> typeIds = viewModel.Types?.Select(q => q.Value).ToList();
            List<string> definitionIds = viewModel.Definitions?.Select(q => q.Value).ToList();
            List<string> roomIds = viewModel.Rooms?.Select(q => q.Value).ToList();
            List<string> personnelIds = viewModel.Personnel?.Select(q => q.Value).ToList();
            List<string> specificProperties = viewModel.SpecificProperties?
                .SelectMany(q => q.Properties.Select(q => q.Value))
                .ToList();
            List<string> propertyIds = viewModel.CommonProperties?
                .Concat(specificProperties == null ? new List<string>() : specificProperties)
                .ToList();
            List<string> universalProperties = viewModel.CommonProperties.Where(q => ReportHelper.CommonProperties.Contains(q)).ToList();

            var model = await GetFullTemplate(viewModel.Id);

            model.Name = viewModel.Name;
            model.Description = viewModel.Description;
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
            model.IncludeInUse = viewModel.IncludeInUse;
            model.IncludeUnused = viewModel.IncludeUnused;  
            model.IncludeFunctional = viewModel.IncludeFunctional;
            model.IncludeDefect = viewModel.IncludeDefect;
            model.IncludeParent = viewModel.IncludeParent;
            model.IncludeChildren = viewModel.IncludeChildren;
            model.Properties = (propertyIds != null)
                    ? (await _unitOfWork.Properties
                    .FindAll<Property>(q => propertyIds.Contains(q.Name)))
                    .ToList()
                    : new List<Property>();
            model.Header = viewModel.Header;
            model.Footer = viewModel.Footer;
            model.SetSignatories(viewModel.Signatories);
            model.CreatedBy = (await _unitOfWork.TEMSUsers
                    .Find<TEMSUser>(
                        where: q => q.UserName == _identityService.User.Identity.Name
                    )).FirstOrDefault();
            model.CommonProperties = (universalProperties.Count > 0)
                    ? String.Join(" ", universalProperties)
                    : null;

            await _unitOfWork.Save();

            return null;
        }

        public async Task<Report> CreateReport(ReportTemplate template)
        {
            Report report = new();
            report.Id = Guid.NewGuid().ToString();
            report.Template = template.Name;
            report.GeneratedByID = _identityService.GetUserId();
            report.DateGenerated = DateTime.Now;
            report.DBPath = fileHandler.GetDBPath();

            var excelReport = await _reportingService.GenerateReport(template, report.DBPath);
            await CheckForReportsOverflow();
            await _unitOfWork.Reports.Create(report);
            await _unitOfWork.Save();

            return report;
        }

        public async Task<string> RemoveReport(Report report)
        {
            try
            {
                _unitOfWork.Reports.Delete(report);
                fileHandler.DeleteFile(report.DBPath);
                await _unitOfWork.Save();
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> Remove(string templateId)
        {
            var template = await GetTemplate(templateId);
            if (template == null)
                return "Invalid id provided";

            _unitOfWork.ReportTemplates.Delete(template);
            await _unitOfWork.Save();
            return null;
        }

        public async Task<Report> GetReport(string reportId)
        {
            return (await _unitOfWork.Reports
                .Find<Report>(q => q.Id == reportId))
                .FirstOrDefault();
        }

        public async Task<List<ViewGeneratedReportViewModel>> GetLastGeneratedReports(
            int skip = 0,
            int take = int.MaxValue)
        {
            var reports = (await _unitOfWork.Reports
                .Find(
                    include: q => q.Include(q => q.GeneratedBy),
                    select: q => ViewGeneratedReportViewModel.FromModel(q)
                )).ToList();

            return reports;
        }

        public async Task<List<ViewReportTemplateSimplifiedViewModel>> GetReportTemplates(
            int skip = 0,
            int take = int.MaxValue)
        {
            var reportTemplates = (await _unitOfWork.ReportTemplates
                .FindAll<ViewReportTemplateSimplifiedViewModel>(
                    where: q => !q.IsArchieved,
                    skip: skip,
                    take: take,
                    include: q => q
                    .Include(q => q.CreatedBy),
                    select: q => ViewReportTemplateSimplifiedViewModel.FromModel(q)
                )).ToList();

            return reportTemplates;
        } 

        public async Task<ReportTemplate> GetTemplate(string templateId)
        {
            var template = (await _unitOfWork.ReportTemplates
                .Find<ReportTemplate>(q => q.Id == templateId))
                .FirstOrDefault();

            return template;
        }

        public async Task<ReportTemplate> GetFullTemplate(string templateId)
        {
            var template = (await _unitOfWork.ReportTemplates
                .FindAll<ReportTemplate>(
                    where: q => q.Id == templateId,
                    include: q => q
                    .Include(q => q.EquipmentTypes)
                    .ThenInclude(q => q.Properties)
                    .Include(q => q.EquipmentDefinitions)
                    .Include(q => q.Rooms)
                    .Include(q => q.Personnel)
                    .Include(q => q.Properties).ThenInclude(q => q.DataType)
                )).FirstOrDefault();

            return template;
        }

        public async Task CheckForReportsOverflow()
        {
            int totalReports = await _unitOfWork.Reports.Count();
            while (totalReports >= _appSettings.GeneratedReportsHistoryLength)
            {
                var toBeRemoved = (await _unitOfWork.Reports
                    .FindAll<Report>(
                        orderBy: q => q.OrderBy(q => q.DateGenerated)
                    )).First();
                string result = await RemoveReport(toBeRemoved);

                if (result == null)
                    --totalReports;
                else
                    break;
            }
        }

        public async Task<MemoryStream> GetReportMemoryStream(string reportDBPath)
        {
            if (!System.IO.File.Exists(reportDBPath))
                return null;

            var memory = new MemoryStream();
            await using (var stream = new FileStream(reportDBPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return memory;
        }
    }
}
