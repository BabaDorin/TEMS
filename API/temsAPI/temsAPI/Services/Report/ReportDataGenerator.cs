using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ReportGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;
using temsAPI.Helpers;
using temsAPI.ViewModels;
using Property = temsAPI.Data.Entities.EquipmentEntities.Property;

namespace temsAPI.Services.Report
{
    internal class ReportDataGenerator
    {
        IUnitOfWork _unitOfWork;
        UserManager<TEMSUser> _userManager;
        List<string> reportUniversalPropertiesList = new List<string>();


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

            reportUniversalPropertiesList = template.UniversalProperties.Split(' ').ToList();
            for (int i = 0; i < reportUniversalPropertiesList.Count; i++)
            {
                var prop = reportUniversalPropertiesList[i];
                reportUniversalPropertiesList[i] = prop.First().ToString().ToUpper() + prop.Substring(1);
            }

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

            var equipment = await FetchEquipmentItems(template);
            // EXIT

            // BEFREE: Test if this method works. If so, remove the part responsible for 
            // fetching separators.
            IEnumerable<IGrouping<IIdentifiable, Equipment>> groupedItems = null;
            switch (template.SepparateBy)
            {
                case "type":
                    groupedItems = equipment
                       .GroupBy(q => q.EquipmentDefinition.EquipmentType)
                       .ToList();
                    break;
                case "definition":
                    groupedItems = equipment
                        .GroupBy(q => q.EquipmentDefinition)
                        .ToList();
                    break;
                case "room":
                    groupedItems = equipment
                        .GroupBy(q => q.EquipmentAllocations
                        .FirstOrDefault(q1 => q1.DateReturned == null) == null
                            ? null
                            : q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null).Room ?? null);
                    break;
                case "personnel":
                    groupedItems = equipment
                        .GroupBy(q => q.EquipmentAllocations
                        .FirstOrDefault(q1 => q1.DateReturned == null) == null
                            ? null
                            : q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null).Personnel ?? null);
                    break;
            }

            List<ReportItemGroup> reportItemGroups = new List<ReportItemGroup>();
            foreach (var group in groupedItems)
            {
                try
                {
                    List<Equipment> items = group.ToList();
                    ReportItemGroup reportItemGroup = new ReportItemGroup
                    {
                        Name = group.Key?.Identifier,
                        ItemsTable = ItemGroupToDataTable(items, template)
                    };

                    reportItemGroups.Add(reportItemGroup);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return reportItemGroups;
        }

        public DataTable ItemGroupToDataTable(List<Equipment> items, ReportTemplate reportTemplate)
        {
            var itemGroupDataTable = new DataTable();

            // Universal properties to columns
            foreach (string prop in reportUniversalPropertiesList)
                itemGroupDataTable.Columns.Add(prop, prop == "price" ? typeof(double) : typeof(string));

            // Specific properties to columns
            foreach (var prop in reportTemplate.Properties)
            {
                if (itemGroupDataTable.Columns.Contains(prop.DisplayName))
                    continue;
                
                itemGroupDataTable.Columns.Add(prop.DisplayName, prop.DataType.GetNativeType());
            }

            var equipmentProperties = typeof(Equipment).GetProperties();
            foreach (Equipment eq in items)
            {
                var row = itemGroupDataTable.NewRow();
                // Add values for universal properties
                foreach (string prop in reportUniversalPropertiesList)
                {
                    row[prop] = equipmentProperties
                        .First(qu => qu.Name.ToLower() == prop.ToLower())
                        .GetValue(eq);
                }

                // Add values for specific properties
                foreach(Property prop in reportTemplate.Properties)
                {
                    var p = eq.EquipmentDefinition.EquipmentSpecifications
                        .FirstOrDefault(qu => qu.Property.Id == prop.Id);

                    row[prop.DisplayName] = p?.Value;
                }

                itemGroupDataTable.Rows.Add(row);
            }

            return itemGroupDataTable;
        }

        public List<string> FetchSignatories(ReportTemplate template)
        {
            return template.Signatories?.Select(q => q.Name).ToList();
        }

        public async Task<List<Equipment>> FetchEquipmentItems(ReportTemplate template)
        {
            Expression<Func<Equipment, bool>> mainExpression = q => !q.IsArchieved;
            Expression<Func<Equipment, bool>> typeFilter = null;
            Expression<Func<Equipment, bool>> definitionFilter = null;
            Expression<Func<Equipment, bool>> personnelFilter = null;
            Expression<Func<Equipment, bool>> roomFilter = null;

            // Build types filter
            if (template.EquipmentTypes != null && template.EquipmentTypes.Count > 0)
                typeFilter = q => template.EquipmentTypes.Contains(q.EquipmentDefinition.EquipmentType);

            // Build definition filter
            if (template.EquipmentDefinitions != null && template.EquipmentDefinitions.Count > 0)
                definitionFilter = q => template.EquipmentDefinitions.Contains(q.EquipmentDefinition);

            // Build personnel filter
            if (template.Personnel != null && template.Personnel.Count > 0)
                personnelFilter = q => template.Personnel.Contains(
                    q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null) == null
                    ? null
                    : q.EquipmentAllocations.FirstOrDefault(q1 => q1.DateReturned == null).Personnel);

            // Build roomFilter
            if (template.Rooms != null && template.Rooms.Count > 0)
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

            List<Equipment> equipment = null;
            try
            {
                equipment = (await _unitOfWork.Equipments
                .Find<Equipment>(
                    include: q => q
                    .Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentType)
                    .Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentSpecifications)
                    .ThenInclude(q => q.Property)
                    .Include(q => q.EquipmentAllocations.Where(q1 => q1.DateReturned == null)),
                    where: mainExpression
                )).ToList();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            
            return equipment;
        }

        public async Task<List<Option>> FetchSeparators(ReportTemplate template)
        {
            // Note: Sepparate by options might differ for another subject.
            // Only equipment subject is treated for now.
            if (template.Subject.ToLower() != "equipment")
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
