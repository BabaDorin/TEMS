using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.LibraryEntities
{
    // BEFREE: Implement library folder
    public class LibraryFolder
    {
        [Key] [MaxLength(150)]
        public string Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        List<LibraryItem> LibraryItems { get; set; } = new List<LibraryItem>();
    }
}
