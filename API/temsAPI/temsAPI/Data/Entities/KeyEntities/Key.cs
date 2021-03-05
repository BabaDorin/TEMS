﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.KeyEntities
{
    public class Key
    {
        [Key]
        public string Id { get; set; }

        public string Identifier { get; set; }

#nullable enable
        public int? Copies { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }

        public virtual ICollection<KeyAllocation> KeyAllocations { get; set; }
    }
}
