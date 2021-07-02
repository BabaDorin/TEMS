﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.UserEntities
{
    public class Privilege
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

#nullable enable

        [MaxLength(100)]
        public string? Identifier { get; set; }

        [MaxLength(250)]
        public string? Description { get; set; }
#nullable disable
    }
}
