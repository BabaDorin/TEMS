using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.LibraryEntities
{
    public class LibraryFolder
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        List<LibraryItem> LibraryItems { get; set; } = new List<LibraryItem>();
    }
}
