﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities
{
    public class DataType
    {
        [Key]
        public string ID { get; set; }

        public string Name { get; set; }
    }
}
