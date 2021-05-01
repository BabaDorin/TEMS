using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using ReportGenerator.Models;
using ReportGenerator.Services;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.Report;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Services
{
    public class ReportingService
    {
        IUnitOfWork _unitOfWork;
        UserManager<TEMSUser> _userManager;

        public ReportingService(IUnitOfWork unitOfWork, UserManager<TEMSUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public FileInfo GenerateReport(ReportTemplate template)
        {
            List<Equipment> equipment = new List<Equipment>();
            var reportData = GenerateReportData(template);
            var reportGenerator = new ReportGenerator.Services.ReportGenerator();
            return reportGenerator.GenerateReport(reportData);
        }

        private ReportData GenerateReportData(ReportTemplate template)
        {
            var reportData = new ReportData
            {
                Footer = template.Footer,
                Header = template.Header,
                Name = template.Name,
                ReportItemGroups = new List<ReportItemGroup>()
            };

            // Test report item group => Includes all equipment records
            System.Linq.Expressions.Expression<Func<Equipment, bool>> mainFilter =
                q => !q.IsArchieved;

            reportData.ReportItemGroups.Add(new ReportItemGroup
            {
                Name = "Test item group",
                ReportItemGroupSignatories = new List<string>() { "Baba Dorin" },
                ItemsTable = FetchItems<Equipment>(template, mainFilter)
            });

            return reportData;
        }

        private DataTable FetchItems<T>(
            ReportTemplate reportTemplate,
            System.Linq.Expressions.Expression<Func<T, bool>> lambda)
        {
            
            return new DataTable();
        }
    }
}
