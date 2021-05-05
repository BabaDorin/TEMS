using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.Report
{
    public class Report
    {
        public string Id { get; set; }
        public string Template { get; set; } // The name of the template when the report has been generated
        public DateTime DateGenerated { get; set; }
        public string DBPath { get; set; }
        [ForeignKey("GeneratedByID")]
        public TEMSUser GeneratedBy { get; set; }
        public string GeneratedByID { get; set; }
    }
}
