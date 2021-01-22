﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Room
    {
        [Key]
        public string ID { get; set; }

        public string Identifier { get; set; }
#nullable enable
        public string? Description { get; set; }
        public int? Floor { get; set; }
#nullable disable
    }
}
