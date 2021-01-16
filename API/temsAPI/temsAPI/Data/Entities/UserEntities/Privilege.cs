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
        public string ID { get; set; }

        public string Identifier { get; set; }
        public string Description { get; set; }
    }
}
