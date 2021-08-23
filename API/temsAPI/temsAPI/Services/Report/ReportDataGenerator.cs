using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
using temsAPI.Helpers.Filters;
using temsAPI.Helpers.ReportHelpers;
using temsAPI.Services.EquipmentManagementHelpers;
using Property = temsAPI.Data.Entities.EquipmentEntities.Property;

namespace temsAPI.Services.Report
{
    public class ReportDataGenerator
    {
        List<string> reportCommonPropertiesList = new List<string>();
        IEquipmentFetcher _equipmentFetcher;

        public ReportDataGenerator(
            IEquipmentFetcher equipmentFetcher)
        {
            _equipmentFetcher = equipmentFetcher;
        }

        /// <summary>
        /// Fetches neceesary information and builds a ReportData instance based on the provided template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<ReportData> GenerateReportData(ReportTemplate template, string generatedBy)
        {
            var reportData = new ReportData
            {
                GeneratedBy = generatedBy,
                Footer = template.Footer,
                Header = template.Header,
                Name = template.Name,
                ReportItemGroups = new List<ReportItemGroup>(),
                Signatories = FetchSignatories(template)
            };

            reportCommonPropertiesList = template.CommonProperties
                .Split(' ')
                .Select(q => q.First().ToString().ToUpper() + q.Substring(1))
                .ToList();

            reportData.ReportItemGroups = await GenerateReportItemGroups(template);

            return reportData;
        }

        /// <summary>
        /// Fetches equipment and packages it into a list of ItemGroups
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public async Task<List<ReportItemGroup>> GenerateReportItemGroups(
            ReportTemplate template)
        {
            //1 => build the main lambda exp and fetch all respective items 
            //2 => sepparate fetched items into multiple report item groups.

            // Executing step 1:
            // Build the main lambda expression, based on template.

            var equipment = await FetchEquipmentItems(template);

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

        /// <summary>
        /// Converts a list of equipment to a dataTable object
        /// </summary>
        /// <param name="items"></param>
        /// <param name="reportTemplate"></param>
        /// <returns></returns>
        public DataTable ItemGroupToDataTable(List<Equipment> items, ReportTemplate reportTemplate)
        {
            var itemGroupDataTable = new DataTable();

            // Universal properties to columns
            foreach (string prop in reportCommonPropertiesList)
                itemGroupDataTable.Columns.Add(prop, ReportHelper.GetCommonPropertyType(prop));

            // Specific properties to columns
            foreach (var prop in reportTemplate.Properties)
            {
                if (itemGroupDataTable.Columns.Contains(prop.DisplayName))
                    continue;
                
                itemGroupDataTable.Columns.Add(prop.DisplayName, prop.DataType.GetNativeType());
            }

            foreach (Equipment eq in items)
            {
                var row = itemGroupDataTable.NewRow();
                // Add values for universal properties
                foreach (string prop in reportCommonPropertiesList)
                {
                    row[prop] = ReportHelper.GetCommonPropertyValueProvider(prop, eq).GetValue(eq) ?? DBNull.Value;
                }

                // Add values for specific properties
                foreach(Property prop in reportTemplate.Properties)
                {
                    if (row[prop.DisplayName] != DBNull.Value)
                        continue; // There is already something there so we won't override. (It happens).#

                    var p = eq.EquipmentDefinition.EquipmentSpecifications
                        .FirstOrDefault(qu => qu.Property.Id == prop.Id);

                    row[prop.DisplayName] = p == null ? DBNull.Value : p.Value;
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
            var filter = new TemplateEquipmentFilterBuilder().GetFilter(template);
            return (await _equipmentFetcher.Fetch(filter)).ToList();
        }
    }
}
