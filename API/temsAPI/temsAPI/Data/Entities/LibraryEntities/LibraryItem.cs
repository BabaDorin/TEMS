using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.UserEntities;

namespace temsAPI.Data.Entities.LibraryEntities
{
    public class LibraryItem
    {
        [Key]
        public string Id { get; set; }

        public string ActualName { get; set; }
        public string DisplayName { get; set; }
        public string DbPath { get; set; }
        public double FileSize { get; set; }
        public int Downloads { get; set; }

        public DateTime DateUploaded { get; set; }

#nullable enable
        public string? Description { get; set; }

        [ForeignKey("LibraryFolderId")]
        public LibraryFolder? LibraryFolder { get; set; }
        public string? LibraryFolderId { get; set; }

        [ForeignKey("UploadedById")]
        public TEMSUser? UploadedBy { get; set; }
        public string? UploadedById { get; set; }
#nullable disable
    }
}
