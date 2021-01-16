﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(EquipmentTypeID))]
    public class EquipmentSpecifications
    {
        [Key]
        public string EquipmentTypeID { get; set; }
        public EquipmentType EquipmentType { get; set; }

        [ForeignKey("PropertyID")]
        public Property Property { get; set; }
        public string PropertyID { get; set; }

        public string Value { get; set; }
    }
}