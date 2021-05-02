using Microsoft.AspNetCore.Identity;
using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.ViewModels;

namespace temsAPI.Services.Report
{
    internal class ReportDataGenerator
    {
        IUnitOfWork _unitOfWork;
        UserManager<TEMSUser> _userManager;

        public ReportDataGenerator(IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<ReportData> GenerateReportData(ReportTemplate template)
        {
            var reportData = new ReportData
            {
                Footer = template.Footer,
                Header = template.Header,
                Name = template.Name,
                ReportItemGroups = new List<ReportItemGroup>(),
                Signatories = FetchSignatories(template)
            };

            List<Option> separators = await FetchSeparators(template);
            if (separators.Count == 0)
                separators.Add(new Option());

            foreach (Option separator in separators)
            {
                reportData.ReportItemGroups.Add(await GenerateReportItemGroup(template, separator));
            }

            return reportData;
        }

        public async Task<ReportItemGroup> GenerateReportItemGroup(ReportTemplate template, Option separator)
        {
            ReportItemGroup reportItemGroup = new();
            reportItemGroup.Name = separator.Label;

            // Build data table, according to template and separator
            reportItemGroup.ItemsTable = new DataTable();
            return reportItemGroup;
        }

        public List<string> FetchSignatories(ReportTemplate template)
        {
            return template.Signatories.Select(q => q.Name).ToList();
        }

        public async Task<List<Option>> FetchSeparators(ReportTemplate template)
        {
            // Note: Sepparate by options might differ for another subject.
            // Only equipment subject is treated for now.
            if (template.Subject != "Equipment")
                throw new Exception("Only Equipment subject is supported at the moment.");

            // Sepparate by possible values: "none", "room", "personnel", "type", "definition" 
            switch (template.SepparateBy)
            {
                case "room":
                    return (await _unitOfWork.Rooms
                        .Find(
                            where: q => !q.IsArchieved,
                            select: q => new Option
                            {
                                Label = q.Identifier,
                                Value = q.Id
                            })).ToList();
                case "personnel":
                    return (await _unitOfWork.Personnel
                        .Find(
                            where: q => !q.IsArchieved,
                            select: q => new Option
                            {
                                Label = q.Name,
                                Value = q.Id
                            }
                        )).ToList();
                case "type":
                    return (await _unitOfWork.EquipmentTypes
                        .Find(
                            where: q => !q.IsArchieved,
                            select: q => new Option
                            {
                                Label = q.Name,
                                Value = q.Id
                            }
                        )).ToList();
                case "definition":
                    return (await _unitOfWork.EquipmentDefinitions
                        .Find(
                            where: q => !q.IsArchieved,
                            select: q => new Option
                            {
                                Value = q.Identifier,
                                Label = q.Id
                            }
                        )).ToList();
                default:
                    return new List<Option>();
            }
        }

        public DataTable FetchItems<T>(
            ReportTemplate reportTemplate,
            System.Linq.Expressions.Expression<Func<T, bool>> lambda)
        {
            return new DataTable();
        }
    }
}
