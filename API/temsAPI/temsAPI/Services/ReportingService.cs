using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

        public void GenerateReport(ReportTemplate template)
        {
            List<Equipment> equipment = new List<Equipment>();
            var reportCriteria = new ReportCriteria();

            //var reportGenerator = new ReportGenerator.Services.ReportGenerator(equipment, reportCriteria);
        }
    }
}
