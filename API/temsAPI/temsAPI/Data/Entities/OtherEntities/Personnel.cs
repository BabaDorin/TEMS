using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class Personnel
    {
        [Key]
        public string ID { get; set; }

        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }

        public string? ImagePath { get; set; }

        // BEFREE: Add multiple Emails and Phone numbers support later if needed.
    }
}
