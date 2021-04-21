using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReportGenerator.Models;
using ReportGenerator.Services;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.Report;

namespace temsAPI.Services
{
    public class ReportingService
    {
        public void GenerateReport(ReportTemplate template)
        {
            List<Equipment> equipment = new List<Equipment>();
            var reportCriteria = new ReportCriteria();

            var reportGenerator = new ReportGenerator.Services.ReportGenerator(equipment, reportCriteria);
        }
    }
}
