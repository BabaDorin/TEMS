using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.UserEntities
{
    public class Privilege
    {
        [Key]
        public string Id { get; set; }

#nullable enable
        public string? Identifier { get; set; }
        public string? Description { get; set; }
#nullable disable
    }
}
