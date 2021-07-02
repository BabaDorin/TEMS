﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.Report
{
    public class Report
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(150)]
        public string Template { get; set; } // The name of the template when the report has been generated

        public DateTime DateGenerated { get; set; }

        [MaxLength(250)]
        public string DBPath { get; set; }

#nullable enable
        [InverseProperty("GeneratedReports")]
        [ForeignKey("GeneratedByID")]
        public TEMSUser? GeneratedBy { get; set; }
        public string? GeneratedByID { get; set; }
#nullable disable
    }
}
