﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    [Index(nameof(DateCreated))]
    public class Announcement
    {
        [Key]
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateClosed { get; set; }

        public string Message { get; set; }

#nullable enable
        [ForeignKey("AuthorID")]
        public TEMSUser? Author { get; set; }
        public string? AuthorID { get; set; }
#nullable disable

        public bool IsArchieved { get; set; }

    }
}
