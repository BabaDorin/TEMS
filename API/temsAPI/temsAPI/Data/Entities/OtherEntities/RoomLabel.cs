using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class RoomLabel
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsArchieved { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
