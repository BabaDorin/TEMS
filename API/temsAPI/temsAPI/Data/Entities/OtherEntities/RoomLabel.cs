using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using temsAPI.Contracts;

namespace temsAPI.Data.Entities.OtherEntities
{
    public class RoomLabel: IArchiveable, IIdentifiable
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateArchieved { get; set; }
        private bool isArchieved;
        public bool IsArchieved
        {
            get { return isArchieved; }
            set { isArchieved = value; DateArchieved = DateTime.Now; }
        }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();

        [NotMapped]
        public string Identifier => Name;
    }
}
