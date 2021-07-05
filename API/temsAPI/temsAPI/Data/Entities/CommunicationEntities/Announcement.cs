﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.CommunicationEntities
{
    public class Announcement
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        public DateTime DateCreated { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(250)]
        public string Message { get; set; }

#nullable enable
        [ForeignKey("AuthorID")]
        public TEMSUser? Author { get; set; }
        public string? AuthorID { get; set; }
#nullable disable
    }
}
