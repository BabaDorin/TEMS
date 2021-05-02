using Microsoft.AspNetCore.Identity;
using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
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

            reportData.ReportItemGroups = await GenerateReportItemGroups(template, separators);

            return reportData;
        }

        public async Task<List<ReportItemGroup>> GenerateReportItemGroups(
            ReportTemplate template, 
            List<Option> separator)
        {
            //1 => build the main lambda exp and fetch all respective items 
            //2 => sepparate fetched items into multiple report item groups.

            // Executing step 1:
            // Build the main lambda expression, based on template.
            if (template.Subject.ToLower() != "equipment")
                throw new Exception("Only Equipment subject supported for now.");

            Expression<Func<Equipment, bool>> mainExpression = q => !q.IsArchieved;
            Expression<Func<Equipment, bool>> typeFilter = null;
            Expression<Func<Equipment, bool>> definitionFilter = null;
            Expression<Func<Equipment, bool>> personnelFilter = null;
            Expression<Func<Equipment, bool>> roomFilter = null;

            // Build types filter
            if(template.EquipmentTypes != null && template.EquipmentTypes.Count > 0)
                typeFilter = q => template.EquipmentTypes.Contains(q.EquipmentDefinition.EquipmentType);

            // Build definition filter
            if (template.EquipmentDefinitions != null && template.EquipmentDefinitions.Count > 0)
                definitionFilter = q => template.EquipmentDefinitions.Contains(q.EquipmentDefinition);

            // Build personnel filter
            if(template.Personnel != null && template.Personnel.Count > 0)
                personnelFilter = q => template.Personnel.Contains(
                    q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null) == null
                    ? null
                    : q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null).Personnel);

            // Build roomFilter
            if (template.Rooms!= null && template.Rooms.Count > 0)
                roomFilter = q => template.Rooms.Contains(
                    q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null) == null
                    ? null
                    : q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null).Room);

            // Merge filters
            mainExpression = ExpressionCombiner.And(
                mainExpression,
                typeFilter,
                definitionFilter,
                personnelFilter,
                roomFilter);

            var equipment = (await _unitOfWork.Equipments
                .Find<Equipment>(
                    where: mainExpression
                )).ToList();

            // Step 2: Having the equipment, build the expression for sepparating them (if needed)
            // ... Working on it

            // Build data table, according to template and separator
            return new List<ReportItemGroup>();
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
