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
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(150)]
        public string ActualName { get; set; }

        [MaxLength(150)]
        public string DisplayName { get; set; }

        [MaxLength(250)]
        public string DbPath { get; set; }

        public double FileSize { get; set; }

        public int Downloads { get; set; }

        public DateTime DateUploaded { get; set; }

#nullable enable
        [MaxLength(250)]
        public string? Description { get; set; }

        [ForeignKey("LibraryFolderId")]
        public LibraryFolder? LibraryFolder { get; set; }

        [MaxLength(150)]
        public string? LibraryFolderId { get; set; }

        [InverseProperty("UploadedLibraryItems")]
        [ForeignKey("UploadedById")]
        public TEMSUser? UploadedBy { get; set; }
        public string? UploadedById { get; set; }
#nullable disable
    }
}
