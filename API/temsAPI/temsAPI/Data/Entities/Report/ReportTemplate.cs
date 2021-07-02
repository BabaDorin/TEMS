﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;
using temsAPI.Data.Entities.EquipmentEntities;
using temsAPI.Data.Entities.OtherEntities;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.Report
{
    public class ReportTemplate: IArchiveableItem
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        [MaxLength(100)]
        public string Subject { get; set; }

        [MaxLength(100)]
        public string SeparateBy { get; set; }

        [MaxLength(1000)]
        public string Header { get; set; }

        [MaxLength(1000)]
        public string Footer { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get
            {
                return isArchieved;
            }
            set
            {
                isArchieved = value;
                DateArchieved = (value)
                    ? DateTime.Now
                    : null;
            }
        }

        // Universal propertill will be stored like this: "property1 property2 property3"
        // These properties are hard-coded because they won't change and we don't want to keep
        // properties into Properties entity that are not related to any type
        // Universal properties are equipment's default properties.

        [MaxLength(1000)]
        public string CommonProperties { get; set; }

#nullable enable
        [InverseProperty("CreatedReportTemplates")]
        [ForeignKey("CreatedById")]
        public TEMSUser? CreatedBy { get; set; }
        public string? CreatedById { get; set; }

        [InverseProperty("ArchivedReportTemplates")]
        [ForeignKey("ArchievedById")]
        public TEMSUser? ArchievedBy { get; set; }
        public string? ArchievedById { get; set; }
#nullable disable

        public List<Personnel> Signatories { get; set; }
        public List<Property> Properties { get; set; }
        public List<EquipmentType> EquipmentTypes { get; set; }
        public List<EquipmentDefinition> EquipmentDefinitions { get; set; }
        public List<Personnel> Personnel { get; set; }
        public List<Room> Rooms { get; set; }

        [NotMapped]
        public string Identifier => Name;
    }
}
