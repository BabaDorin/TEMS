﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.EquipmentEntities
{
    public class Property
    {
        [Key]
        public string ID { get; set; }

        public string Name { get; set; }

#nullable enable
        public string? DisplayName { get; set; }
#nullable disable


        [ForeignKey("DataTypeID")]
        public DataType DataType { get; set; }
#nullable enable
        public string? DataTypeID { get; set; }
#nullable disable
    }
}
