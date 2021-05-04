﻿using Microsoft.AspNetCore.Identity;
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

            reportUniversalPropertiesList = template.CommonProperties.Split(' ').ToList();
            for (int i = 0; i < reportUniversalPropertiesList.Count; i++)
            {
                var prop = reportUniversalPropertiesList[i];
                reportUniversalPropertiesList[i] = prop.First().ToString().ToUpper() + prop.Substring(1);
            }

            reportData.ReportItemGroups = await GenerateReportItemGroups(template);

            return reportData;
        }

        public async Task<List<ReportItemGroup>> GenerateReportItemGroups(
            ReportTemplate template)
        {
            //1 => build the main lambda exp and fetch all respective items 
            //2 => sepparate fetched items into multiple report item groups.

            // Executing step 1:
            // Build the main lambda expression, based on template.
            if (template.Subject.ToLower() != "equipment")
                throw new Exception("Only Equipment subject supported for now.");

            var equipment = await FetchEquipmentItems(template);
            // EXIT

            var separator = ReportHelper.GetSeparator(template);
            var groupedItems = separator.GroupEquipment(equipment);

            List<ReportItemGroup> reportItemGroups = new List<ReportItemGroup>();
            foreach (var group in groupedItems)
            {
                List<Equipment> items = group.ToList();
                ReportItemGroup reportItemGroup = new ReportItemGroup
                {
                    Name = group.Key?.Identifier,
                    ItemsTable = ItemGroupToDataTable(items, template)
                };

                reportItemGroups.Add(reportItemGroup);
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

            var equipment = (await _unitOfWork.Equipments
            .Find<Equipment>(
                include: q => q
                .Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentType)
                .Include(q => q.EquipmentDefinition).ThenInclude(q => q.EquipmentSpecifications)
                .ThenInclude(q => q.Property)
                .Include(q => q.EquipmentAllocations.Where(q1 => q1.DateReturned == null)),
                where: mainExpression
            )).ToList();
            
            return equipment;
        }

        public DataTable FetchItems<T>(
            ReportTemplate reportTemplate,
            System.Linq.Expressions.Expression<Func<T, bool>> lambda)
        {
            return new DataTable();
        }
    }
}
