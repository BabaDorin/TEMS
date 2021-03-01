﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    [Index(nameof(EquipmentDefinitionID))]
    public class EquipmentSpecifications
    {
        // Which properties relates to which equipment types.

        [Key]
        public string ID { get; set; }

        [ForeignKey("EquipmentDefinitionID")]
        public EquipmentDefinition EquipmentDefinition { get; set; }
        public string EquipmentDefinitionID { get; set; }


        [ForeignKey("PropertyID")]
        public Property Property { get; set; }
        public string PropertyID { get; set; }

#nullable enable
        public string? Value { get; set; }
#nullable disable
    }
}