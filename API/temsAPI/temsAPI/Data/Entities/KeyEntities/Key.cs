using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Data.Entities.OtherEntities;

namespace temsAPI.Data.Entities.KeyEntities
{
    public class Key
    {
        [Key]
        public string Id { get; set; }

        public string Identifier { get; set; }
        //public int Copies { get; set; } = 0;
        public bool IsArchieved { get; set; }

#nullable enable
        public string? Description { get; set; }
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }
        public string? RoomId { get; set; }
#nullable disable

        public virtual ICollection<KeyAllocation> KeyAllocations { get; set; } = new List<KeyAllocation>();
    }
}
