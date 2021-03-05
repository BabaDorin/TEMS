using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class ToDo
    {
        [Key]
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateClosed { get; set; }
#nullable enable
        public string? Text { get; set; }
#nullable disable


        public bool Urgent { get; set; }

#nullable enable
        [ForeignKey("CreatedByID")]
        public TEMSUser? CreatedBy { get; set; }
        public string? CreatedByID { get; set; }
#nullable disable
    }
}
